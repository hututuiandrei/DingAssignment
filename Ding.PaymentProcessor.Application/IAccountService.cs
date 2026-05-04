using Ding.PaymentProcessor.Domain;

namespace Ding.PaymentProcessor.Application;

public interface IAccountService
{
    void Deposit(Amount amount);

    void Withdraw(Amount amount);

    void PrintStatement();
}

