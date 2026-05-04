namespace Ding.PaymentProcessor.Api.Dtos;

public record AccountDto(Guid Id, string Owner, decimal Balance, string Currency);
