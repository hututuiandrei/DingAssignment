namespace Ding.PaymentProcessor.Domain;

public sealed class Account
{
    private readonly List<Transaction> _transactions = [];

    public Guid Id { get; private set; }
    public string Owner { get; private set; }
    public Amount Balance { get; set; }

    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    private Account() { }

    public static Account Open(string owner, string currency)
    {
        if (string.IsNullOrWhiteSpace(owner))
            throw new ArgumentException("Owner cannot be empty.", nameof(owner));

        return new Account
        {
            Id = Guid.NewGuid(),
            Owner = owner,
            Balance = Amount.Zero(currency)
        };
    }

    public void Deposit(Amount amount)
    {
        if (amount.Value <= 0)
            throw new ArgumentException("Deposit amount must be positive.");

        Balance = Balance.Add(amount);
        _transactions.Add(Transaction.Create(Id, amount, TransactionType.Deposit));
    }

    public void Withdraw(Amount amount)
    {
        if (amount.Value <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.");
        if (Balance.Value < amount.Value)
            throw new InvalidOperationException("Insufficient funds.");

        Balance = Balance.Subtract(amount);
        _transactions.Add(Transaction.Create(Id, amount, TransactionType.Withdrawal));
    }
}