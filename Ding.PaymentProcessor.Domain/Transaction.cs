namespace Ding.PaymentProcessor.Domain;

public sealed class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Amount Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(Guid accountId, Amount amount, TransactionType type)
    {
        return new Transaction
        {
            AccountId = accountId,
            Amount = amount,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };
    }
}