namespace Ding.PaymentProcessor.Domain;

public sealed record Amount
{
    public decimal Value { get; init; }
    public string Currency { get; init; }

    private Amount() { }

    public static Amount Of(decimal value, string currency)
    {
        if (value < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(value));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));

        return new Amount
        {
            Value = Math.Round(value, 2),
            Currency = currency.ToUpperInvariant()
        };
    }

    public static Amount Zero(string currency) => Of(0, currency);

    public Amount Add(Amount other)
    {
        EnsureSameCurrency(other);
        return Of(Value + other.Value, Currency);
    }

    public Amount Subtract(Amount other)
    {
        EnsureSameCurrency(other);
        return Of(Value - other.Value, Currency);
    }

    private void EnsureSameCurrency(Amount other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot operate on different currencies: {Currency} and {other.Currency}.");
    }

    public static Amount operator +(Amount left, Amount right) => left.Add(right);
    public static Amount operator -(Amount left, Amount right) => left.Subtract(right);
}