using Ding.PaymentProcessor.Api.Dtos;
using Ding.PaymentProcessor.Api.Middlewares;
using Ding.PaymentProcessor.Application;
using Ding.PaymentProcessor.Application.Common;
using Ding.PaymentProcessor.Domain;
using Ding.PaymentProcessor.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<IPaymentProcessorContext, PaymentProcessorContext>(options =>
    options.UseSqlite("Data Source=PaymentProcessor.db"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAccountService>(provider =>
{
    var httpContext = provider.GetRequiredService<IHttpContextAccessor>();
    var dbContext = provider.GetRequiredService<IPaymentProcessorContext>();

    var accountId = Guid.Parse(httpContext.HttpContext!.Request.RouteValues["accountId"]!.ToString()!);

    var account = dbContext.Accounts
        .Include(a => a.Transactions)
        .FirstOrDefault(a => a.Id == accountId)
        ?? throw new KeyNotFoundException($"Account {accountId} not found.");

    return new AccountService(account, dbContext);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database with initial accounts
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentProcessorContext>();
    dbContext.Database.EnsureCreated();
    
    if (!dbContext.Accounts.Any())
    {
        var accounts = Enumerable.Range(1, 10)
            .Select(i => Account.Open($"Account Owner {i}", "USD"))
            .ToList();
        
        dbContext.Accounts.AddRange(accounts);
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapPost("/accounts/{accountId}/deposit", (Guid accountId, DepositRequestDto request, IAccountService service) =>
{
    service.Deposit(Amount.Of(request.Amount, request.Currency));
    return Results.Ok();
});

app.MapPost("/accounts/{accountId}/withdraw", (Guid accountId, WithdrawRequestDto request, IAccountService service) =>
{
    service.Withdraw(Amount.Of(request.Amount, request.Currency));
    return Results.Ok();
});

app.MapGet("/accounts/{accountId}/statement", (Guid accountId, IAccountService service) =>
{
    // Capture the console output of PrintStatement
    var originalOut = Console.Out;
    using var writer = new StringWriter();
    Console.SetOut(writer);

    try
    {
        service.PrintStatement();
    }
    finally
    {
        Console.SetOut(originalOut);
    }

    var statementText = writer.ToString();
    return Results.Text(statementText, "text/plain");
});

app.MapGet("/accounts", (PaymentProcessorContext dbContext) =>
{
    var accounts = dbContext.Accounts
        .Select(a => new AccountDto(a.Id, a.Owner, a.Balance.Value, a.Balance.Currency))
        .ToList();
    
    return Results.Ok(accounts);
});

app.Run();