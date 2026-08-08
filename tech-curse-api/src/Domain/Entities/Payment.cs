using System;
using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.Domain.Entities;

public class Payment
{
    public int PaymentId { get; set; }

    // Relations
    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; }

    // Payment data
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // Active flag used to enforce the unique active-payment-per-enrollment constraint
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    // Optional reference to external provider transaction id
    public string? ExternalTransactionId { get; set; }
}
