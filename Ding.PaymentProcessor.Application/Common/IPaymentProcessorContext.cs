using Ding.PaymentProcessor.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ding.PaymentProcessor.Application.Common;

public interface IPaymentProcessorContext
{
    public DbSet<Account> Accounts { get; }
    public DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
