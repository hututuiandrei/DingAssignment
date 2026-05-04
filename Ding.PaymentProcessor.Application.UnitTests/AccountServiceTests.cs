using Ding.PaymentProcessor.Application;
using Ding.PaymentProcessor.Application.Common;
using Ding.PaymentProcessor.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ding.PaymentProcessor.Application.UnitTests;

[TestFixture]
public class AccountServiceTests
{
    private Mock<IPaymentProcessorContext> _contextMock;
    private Mock<DbSet<Account>> _accountsDbSetMock;

    [SetUp]
    public void SetUp()
    {
        _accountsDbSetMock = new Mock<DbSet<Account>>();
        _contextMock = new Mock<IPaymentProcessorContext>();
        _contextMock.Setup(c => c.Accounts).Returns(_accountsDbSetMock.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    private static Account CreateAccount(decimal initialBalance = 0)
    {
        var account = Account.Open("John Doe", "USD");
        if (initialBalance > 0)
            account.Deposit(Amount.Of(initialBalance, "USD"));
        return account;
    }

    private AccountService CreateService(Account account) => new(account, _contextMock.Object);

    // Deposit
    [Test]
    public void Deposit_ShouldIncreaseBalance()
    {
        // Arrange
        var account = CreateAccount();
        var service = CreateService(account);

        // Act
        service.Deposit(Amount.Of(100m, "USD"));

        // Assert
        Assert.That(account.Balance.Value, Is.EqualTo(100m));
    }

    [Test]
    public void Deposit_ShouldAddTransaction()
    {
        // Arrange
        var account = CreateAccount();
        var service = CreateService(account);

        // Act
        service.Deposit(Amount.Of(100m, "USD"));

        // Assert
        Assert.That(account.Transactions, Has.Count.EqualTo(1));
        Assert.That(account.Transactions[0].Type, Is.EqualTo(TransactionType.Deposit));
    }

    [Test]
    public void Deposit_ShouldCallSaveChanges()
    {
        // Arrange
        var account = CreateAccount();
        var service = CreateService(account);

        // Act
        service.Deposit(Amount.Of(100m, "USD"));

        // Assert
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Deposit_ShouldThrow_WhenAmountIsZero()
    {
        // Arrange
        var account = CreateAccount();
        var service = CreateService(account);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.Deposit(Amount.Of(0m, "USD")));
    }

    // Withdraw
    [Test]
    public void Withdraw_ShouldDecreaseBalance()
    {
        // Arrange
        var account = CreateAccount(initialBalance: 500m);
        var service = CreateService(account);

        // Act
        service.Withdraw(Amount.Of(200m, "USD"));

        // Assert
        Assert.That(account.Balance.Value, Is.EqualTo(300m));
    }

    [Test]
    public void Withdraw_ShouldAddTransaction()
    {
        // Arrange
        var account = CreateAccount(initialBalance: 500m);
        var service = CreateService(account);

        // Act
        service.Withdraw(Amount.Of(200m, "USD"));

        // Assert
        Assert.That(account.Transactions, Has.Count.EqualTo(2)); // 1 deposit + 1 withdrawal
        Assert.That(account.Transactions.Last().Type, Is.EqualTo(TransactionType.Withdrawal));
    }

    [Test]
    public void Withdraw_ShouldCallSaveChanges()
    {
        // Arrange
        var account = CreateAccount(initialBalance: 500m);
        var service = CreateService(account);

        // Act
        service.Withdraw(Amount.Of(200m, "USD"));

        // Assert
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Withdraw_ShouldThrow_WhenInsufficientFunds()
    {
        // Arrange
        var account = CreateAccount(initialBalance: 100m);
        var service = CreateService(account);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.Withdraw(Amount.Of(500m, "USD")));
    }

    [Test]
    public void Withdraw_ShouldAttachAccount()
    {
        // Arrange
        var account = CreateAccount(initialBalance: 500m);
        var service = CreateService(account);

        // Act
        service.Withdraw(Amount.Of(200m, "USD"));

        // Assert
        _accountsDbSetMock.Verify(d => d.Attach(account), Times.Once);
    }

    // PrintStatement
    [Test]
    public void PrintStatement_ShouldPrintHeader()
    {
        // Arrange
        var account = CreateAccount();
        var service = CreateService(account);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.PrintStatement();

        // Assert
        var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines[0], Is.EqualTo("Date || Amount || Balance"));
    }

    [Test]
    public void PrintStatement_ShouldPrintCorrectNumberOfLines()
    {
        // Arrange
        var account = CreateAccount();
        account.Deposit(Amount.Of(1000m, "USD"));
        account.Deposit(Amount.Of(2000m, "USD"));
        account.Withdraw(Amount.Of(500m, "USD"));
        var service = CreateService(account);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.PrintStatement();

        // Assert
        var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines, Has.Length.EqualTo(4)); // header + 3 transactions
    }

    [Test]
    public void PrintStatement_ShouldShowNegativeAmountForWithdrawals()
    {
        // Arrange
        var account = CreateAccount(initialBalance: 1000m);
        account.Withdraw(Amount.Of(500m, "USD"));
        var service = CreateService(account);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.PrintStatement();

        // Assert
        var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Any(l => l.Contains("-500")), Is.True);
    }

    [TearDown]
    public void TearDown()
    {
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }
}