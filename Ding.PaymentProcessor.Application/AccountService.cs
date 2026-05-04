using Ding.PaymentProcessor.Application.Common;
using Ding.PaymentProcessor.Domain;

namespace Ding.PaymentProcessor.Application;

public class AccountService : IAccountService
{
    private readonly Account _account;
    private readonly IPaymentProcessorContext _context;

    public AccountService(Account account, IPaymentProcessorContext context)
    {
        _account = account;
        _context = context;
    }

    public void Deposit(Amount amount)
    {
        _account.Deposit(amount);
        _context.SaveChangesAsync(CancellationToken.None).Wait();
    }

    public void Withdraw(Amount amount)
    {
        _context.Accounts.Attach(_account);

        _account.Withdraw(amount);
        _context.SaveChangesAsync(CancellationToken.None).Wait();
    }

    public void PrintStatement()
    {
        Console.WriteLine("Date || Amount || Balance");

        var runningBalance = Amount.Zero(_account.Balance.Currency);

        var lines = new List<string>();

        foreach (var transaction in _account.Transactions.OrderBy(t => t.CreatedAt))
        {
            if (transaction.Type == TransactionType.Deposit)
            {
                runningBalance = runningBalance.Add(transaction.Amount);
                lines.Add($"{transaction.CreatedAt:dd/MM/yyyy} || {transaction.Amount.Value} || {runningBalance.Value}");
            }
            else
            {
                runningBalance = runningBalance.Subtract(transaction.Amount);
                lines.Add($"{transaction.CreatedAt:dd/MM/yyyy} || -{transaction.Amount.Value} || {runningBalance.Value}");
            }
        }

        foreach (var line in lines.AsEnumerable().Reverse())
            Console.WriteLine(line);
    }
}