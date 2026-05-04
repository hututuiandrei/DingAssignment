using Ding.PaymentProcessor.Application.Common;
using Ding.PaymentProcessor.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ding.PaymentProcessor.Infrastructure;

public class PaymentProcessorContext : DbContext, IPaymentProcessorContext
{
    public DbSet<Account> Accounts { get; set;}
    public DbSet<Transaction> Transactions { get; set;}

    public PaymentProcessorContext(DbContextOptions<PaymentProcessorContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentProcessorContext).Assembly);
    }
}