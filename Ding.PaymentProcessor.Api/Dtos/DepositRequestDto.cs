namespace Ding.PaymentProcessor.Api.Dtos;

public record DepositRequestDto(decimal Amount, string Currency);