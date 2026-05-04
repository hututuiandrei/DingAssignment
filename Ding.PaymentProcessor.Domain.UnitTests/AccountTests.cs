namespace Ding.PaymentProcessor.Domain.UnitTests;

public class AccountTests
{
    [Test]
    public void Open_WithValidOwner_CreatesAccountWithZeroBalance()
    {
        // Arrange & Act
        var account = Account.Open("John Doe", "USD");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(account.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(account.Owner, Is.EqualTo("John Doe"));
            Assert.That(account.Balance.Value, Is.EqualTo(0));
            Assert.That(account.Balance.Currency, Is.EqualTo("USD"));
            Assert.That(account.Transactions, Is.Empty);
        });
    }

    [Test]
    [TestCase("")]
    [TestCase("   ")]
    public void Open_WithInvalidOwner_ThrowsArgumentException(string owner)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Account.Open(owner, "USD"));
    }

    [Test]
    public void Deposit_WithPositiveAmount_IncreasesBalance()
    {
        // Arrange
        var account = Account.Open("John Doe", "USD");
        var depositAmount = Amount.Of(100, "USD");

        // Act
        account.Deposit(depositAmount);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(account.Balance.Value, Is.EqualTo(100));
            Assert.That(account.Transactions.Count, Is.EqualTo(1));
            Assert.That(account.Transactions[0].Type, Is.EqualTo(TransactionType.Deposit));
            Assert.That(account.Transactions[0].Amount.Value, Is.EqualTo(100));
        });
    }

    [Test]
    public void Deposit_MultipleDeposits_AccumulatesBalance()
    {
        // Arrange
        var account = Account.Open("John Doe", "USD");

        // Act
        account.Deposit(Amount.Of(100, "USD"));
        account.Deposit(Amount.Of(50, "USD"));
        account.Deposit(Amount.Of(25, "USD"));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(account.Balance.Value, Is.EqualTo(175));
            Assert.That(account.Transactions.Count, Is.EqualTo(3));
        });
    }

    [Test]
    [TestCase(0)]
    [TestCase(-50)]
    public void Deposit_WithNegativeOrZeroAmount_ThrowsArgumentException(decimal amount)
    {
        // Arrange
        var account = Account.Open("John Doe", "USD");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => account.Deposit(Amount.Of(amount, "USD")));
    }

    [Test]
    public void Withdraw_WithValidAmount_DecreasesBalance()
    {
        // Arrange
        var account = Account.Open("John Doe", "USD");
        account.Deposit(Amount.Of(100, "USD"));

        // Act
        account.Withdraw(Amount.Of(30, "USD"));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(account.Balance.Value, Is.EqualTo(70));
            Assert.That(account.Transactions.Count, Is.EqualTo(2));
            Assert.That(account.Transactions[1].Type, Is.EqualTo(TransactionType.Withdrawal));
            Assert.That(account.Transactions[1].Amount.Value, Is.EqualTo(30));
        });
    }

    [Test]
    public void Withdraw_WithInsufficientFunds_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = Account.Open("John Doe", "USD");
        account.Deposit(Amount.Of(50, "USD"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => account.Withdraw(Amount.Of(100, "USD")));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-50)]
    public void Withdraw_WithNegativeOrZeroAmount_ThrowsArgumentException(decimal amount)
    {
        // Arrange
        var account = Account.Open("John Doe", "USD");
        account.Deposit(Amount.Of(100, "USD"));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => account.Withdraw(Amount.Of(amount, "USD")));
    }

    [Test]
    public void Withdraw_ExactBalance_ResultsInZeroBalance()
    {
        // Arrange
        var account = Account.Open("John Doe", "USD");
        account.Deposit(Amount.Of(100, "USD"));

        // Act
        account.Withdraw(Amount.Of(100, "USD"));

        // Assert
        Assert.That(account.Balance.Value, Is.EqualTo(0));
    }
}
