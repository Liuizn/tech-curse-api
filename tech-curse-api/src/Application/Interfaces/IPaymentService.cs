using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Interfaces;

public interface IPaymentService
{    
    Task<PagedResultDto<PaymentOutputDto>> GetPagedAsync(PaginationParamsDto searchParams);
    Task<PaymentOutputDto?> GetByIdAsync(int id);
    Task<PagedResultDto<PaymentOutputDto>?> GetByStudentIdAsync(int studentId, PaginationParamsDto searchParams);
    Task<IEnumerable<PaymentOutputDto>?> GetByEnrollmentIdAsync(int enrollmentId);
    Task<PaymentOutputDto> CreatePaymentAsync(CreatePaymentDto dto);
    Task<ProcessPaymentOutputDto> ProcessPaymentAsync(string idempotencyKey, ProcessPaymentDto dto);
    Task<RefundPaymentOutputDto> RefundPaymentAsync(string idempotencyKey, RefundPaymentDto dto);
}
