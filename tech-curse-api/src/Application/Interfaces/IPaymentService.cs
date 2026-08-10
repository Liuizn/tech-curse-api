using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentOutputDto> CreateAsync(PaymentPostDto dto);
    Task<PagedResultDto<PaymentOutputDto>> GetPagedAsync(PaginationParamsDto searchParams);
    Task<PaymentOutputDto?> GetByIdAsync(int id);
    Task<PagedResultDto<PaymentOutputDto>> GetByStudentIdAsync(int studentId, PaginationParamsDto searchParams);
    Task<IEnumerable<PaymentOutputDto>> GetByEnrollmentIdAsync(int enrollmentId);
    Task<PaymentOutputDto?> ProcessPaymentAsync(int id, ProcessPaymentDto dto);
    Task<PaymentOutputDto?> RefundPaymentAsync(int id, RefundPaymentDto dto);
}
