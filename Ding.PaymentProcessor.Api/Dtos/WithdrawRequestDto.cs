namespace Ding.PaymentProcessor.Api.Dtos;

public record WithdrawRequestDto(decimal Amount, string Currency);