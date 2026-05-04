using Ding.PaymentProcessor.Domain;

namespace Ding.PaymentProcessor.Domain.UnitTests;

public class AmountTests
{
    [Test]
    public void Of_WithValidValues_CreatesAmount()
    {
        // Act
        var amount = Amount.Of(100.50m, "USD");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(amount.Value, Is.EqualTo(100.50m));
            Assert.That(amount.Currency, Is.EqualTo("USD"));
        });
    }

    [Test]
    public void Of_WithLowercaseCurrency_ConvertsToUppercase()
    {
        // Act
        var amount = Amount.Of(100, "usd");

        // Assert
        Assert.That(amount.Currency, Is.EqualTo("USD"));
    }

    [Test]
    public void Of_WithNegativeValue_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Amount.Of(-50, "USD"));
    }

    [Test]
    [TestCase("")]
    [TestCase("   ")]
    public void Of_WithInvalidCurrency_ThrowsArgumentException(string currency)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Amount.Of(100, currency));
    }

    [Test]
    public void Of_RoundsValueToTwoDecimalPlaces()
    {
        // Act
        var amount = Amount.Of(100.556m, "USD");

        // Assert
        Assert.That(amount.Value, Is.EqualTo(100.56m));
    }

    [Test]
    public void Zero_CreatesAmountWithZeroValue()
    {
        // Act
        var amount = Amount.Zero("EUR");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(amount.Value, Is.EqualTo(0));
            Assert.That(amount.Currency, Is.EqualTo("EUR"));
        });
    }

    [Test]
    public void Add_WithSameCurrency_AddsAmounts()
    {
        // Arrange
        var amount1 = Amount.Of(100, "USD");
        var amount2 = Amount.Of(50, "USD");

        // Act
        var result = amount1.Add(amount2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value, Is.EqualTo(150));
            Assert.That(result.Currency, Is.EqualTo("USD"));
        });
    }

    [Test]
    public void Add_WithDifferentCurrency_ThrowsInvalidOperationException()
    {
        // Arrange
        var amount1 = Amount.Of(100, "USD");
        var amount2 = Amount.Of(50, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => amount1.Add(amount2));
    }

    [Test]
    public void Subtract_WithSameCurrency_SubtractsAmounts()
    {
        // Arrange
        var amount1 = Amount.Of(100, "USD");
        var amount2 = Amount.Of(30, "USD");

        // Act
        var result = amount1.Subtract(amount2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value, Is.EqualTo(70));
            Assert.That(result.Currency, Is.EqualTo("USD"));
        });
    }

    [Test]
    public void Subtract_WithDifferentCurrency_ThrowsInvalidOperationException()
    {
        // Arrange
        var amount1 = Amount.Of(100, "USD");
        var amount2 = Amount.Of(30, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => amount1.Subtract(amount2));
    }

    [Test]
    public void AddOperator_WithSameCurrency_AddsAmounts()
    {
        // Arrange
        var amount1 = Amount.Of(100, "USD");
        var amount2 = Amount.Of(50, "USD");

        // Act
        var result = amount1 + amount2;

        // Assert
        Assert.That(result.Value, Is.EqualTo(150));
    }

    [Test]
    public void SubtractOperator_WithSameCurrency_SubtractsAmounts()
    {
        // Arrange
        var amount1 = Amount.Of(100, "USD");
        var amount2 = Amount.Of(30, "USD");

        // Act
        var result = amount1 - amount2;

        // Assert
        Assert.That(result.Value, Is.EqualTo(70));
    }
}
