using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;

namespace tech_curse_api.src.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICacheService _cacheService;

    private const string PAYMENT_LIST_PREFIX = "payments:list:";
    private const string PAYMENT_ITEM_PREFIX = "payments:item:";
    private const string PAYMENT_BY_STUDENT_PREFIX = "payments:student:";
    private const string PAYMENT_BY_ENROLLMENT_PREFIX = "payments:enrollment:";

    public PaymentService(IPaymentRepository paymentRepository, ICacheService cacheService)
    {
        _paymentRepository = paymentRepository;
        _cacheService = cacheService;
    }

    public async Task<PaymentOutputDto> CreateAsync(PaymentPostDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResultDto<PaymentOutputDto>> GetPagedAsync(PaginationParamsDto searchParams)
    {
        var cacheKey = $"{PAYMENT_LIST_PREFIX}page:{searchParams.PageNumber}:size:{searchParams.PageSize}:sort:{searchParams.SortBy}_{searchParams.SortDirection}";

        var cachedResult = await _cacheService.GetAsync<PagedResultDto<PaymentOutputDto>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var (items, totalCount) = await _paymentRepository.GetPagedAsync(searchParams);

        var dtos = items.Select(p => new PaymentOutputDto(
            p.PaymentId,
            p.EnrollmentId,
            p.StudentId,
            p.Amount,
            p.Status,
            p.IsActive,
            p.CreatedAt,
            p.PaidAt,
            p.ExternalTransactionId
        ));

        var result = new PagedResultDto<PaymentOutputDto>(
            dtos,
            totalCount,
            searchParams.PageNumber,
            searchParams.PageSize
        );

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<PaymentOutputDto?> GetByIdAsync(int id)
    {
        var cacheKey = $"{PAYMENT_ITEM_PREFIX}{id}";

        var cached = await _cacheService.GetAsync<PaymentOutputDto>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null) return null;

        var dto = new PaymentOutputDto(
            payment.PaymentId,
            payment.EnrollmentId,
            payment.StudentId,
            payment.Amount,
            payment.Status,
            payment.IsActive,
            payment.CreatedAt,
            payment.PaidAt,
            payment.ExternalTransactionId
        );

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15));

        return dto;
    }

    public async Task<PagedResultDto<PaymentOutputDto>> GetByStudentIdAsync(int studentId, PaginationParamsDto searchParams)
    {
        var cacheKey = $"{PAYMENT_BY_STUDENT_PREFIX}id:{studentId}:page:{searchParams.PageNumber}:size:{searchParams.PageSize}:sort:{searchParams.SortBy}_{searchParams.SortDirection}";

        var cached = await _cacheService.GetAsync<PagedResultDto<PaymentOutputDto>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var (items, totalCount) = await _paymentRepository.GetByStudentIdAsync(studentId, searchParams);

        var dtos = items.Select(p => new PaymentOutputDto(
            p.PaymentId,
            p.EnrollmentId,
            p.StudentId,
            p.Amount,
            p.Status,
            p.IsActive,
            p.CreatedAt,
            p.PaidAt,
            p.ExternalTransactionId
        ));

        var result = new PagedResultDto<PaymentOutputDto>(
            dtos,
            totalCount,
            searchParams.PageNumber,
            searchParams.PageSize
        );

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<IEnumerable<PaymentOutputDto>> GetByEnrollmentIdAsync(int enrollmentId)
    {
        var cacheKey = $"{PAYMENT_BY_ENROLLMENT_PREFIX}{enrollmentId}";

        var cached = await _cacheService.GetAsync<IEnumerable<PaymentOutputDto>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var payments = await _paymentRepository.GetByEnrollmentIdAsync(enrollmentId);

        var dtos = payments.Select(p => new PaymentOutputDto(
            p.PaymentId,
            p.EnrollmentId,
            p.StudentId,
            p.Amount,
            p.Status,
            p.IsActive,
            p.CreatedAt,
            p.PaidAt,
            p.ExternalTransactionId
        )).ToList();

        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15));

        return dtos;
    }

    public async Task<PaymentOutputDto?> ProcessPaymentAsync(int id, ProcessPaymentDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<PaymentOutputDto?> RefundPaymentAsync(int id, RefundPaymentDto dto)
    {
        throw new NotImplementedException();
    }
}
