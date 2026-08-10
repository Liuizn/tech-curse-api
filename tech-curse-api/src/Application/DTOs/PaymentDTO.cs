using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.Application.DTOs;

public record PaymentPostDto(int EnrollmentId, int StudentId, decimal Amount);
public record PaymentOutputDto(int PaymentId, int EnrollmentId, int StudentId, decimal Amount, PaymentStatus Status, bool IsActive, DateTime CreatedAt, DateTime? PaidAt, string? ExternalTransactionId);
public record ProcessPaymentDto(string? ExternalTransactionId);
public record RefundPaymentDto(string? Reason);
