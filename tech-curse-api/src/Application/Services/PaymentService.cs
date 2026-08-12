using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Factory;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Domain.Enums;
using tech_curse_api.src.Domain.Exceptions;
using tech_curse_api.src.Domain.Specifications;

namespace tech_curse_api.src.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IPaymentRepository _paymentRepository;

    private readonly IPaymentGatewayAdapter _paymentGateway;
    private readonly PaymentStrategyFactory _strategyFactory;

    private readonly ICacheService _cacheService;
    private readonly ICurrentUserService _currentUserService;

    private const string PAYMENT_LIST_PREFIX = "payments:list:";
    private const string PAYMENT_ITEM_PREFIX = "payments:item:";
    private const string PAYMENT_BY_STUDENT_PREFIX = "payments:student:";
    private const string PAYMENT_BY_ENROLLMENT_PREFIX = "payments:enrollment:";

    public PaymentService(
        IStudentRepository studentRepository,
        IEnrollmentRepository enrollmentRepository,
        IPaymentRepository paymentRepository,
        IPaymentGatewayAdapter paymentGateway,
        PaymentStrategyFactory strategyFactory,
        ICacheService cacheService,
        ICurrentUserService currentUserService
    ) {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
        _paymentRepository = paymentRepository;

        _paymentGateway = paymentGateway;
        _strategyFactory = strategyFactory;

        _cacheService = cacheService;
        _currentUserService = currentUserService;
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

        await ValidateRoleAcess(payment.Student.IdentityUserId);

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

    public async Task<PagedResultDto<PaymentOutputDto>?> GetByStudentIdAsync(int studentId, PaginationParamsDto searchParams)
    {
        var cacheKey = $"{PAYMENT_BY_STUDENT_PREFIX}id:{studentId}:page:{searchParams.PageNumber}:size:{searchParams.PageSize}:sort:{searchParams.SortBy}_{searchParams.SortDirection}";

        var cached = await _cacheService.GetAsync<PagedResultDto<PaymentOutputDto>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null) return null;

        await ValidateRoleAcess(student.IdentityUserId);

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

    public async Task<IEnumerable<PaymentOutputDto>?> GetByEnrollmentIdAsync(int enrollmentId)
    {
        var cacheKey = $"{PAYMENT_BY_ENROLLMENT_PREFIX}{enrollmentId}";

        var cached = await _cacheService.GetAsync<IEnumerable<PaymentOutputDto>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var payments = await _paymentRepository.GetByEnrollmentIdAsync(enrollmentId);

        var student = payments.FirstOrDefault()?.Enrollment.Student;
        if (student == null) return null;

        await ValidateRoleAcess(student.IdentityUserId);

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

    public async Task<PaymentOutputDto> CreatePaymentAsync(CreatePaymentDto input)
    {
        Enrollment? enrollment = await _enrollmentRepository.GetByIdAsync(input.EnrollmentId);

        if (enrollment == null)
        {
            throw new NotFoundException("Matrícula não encontrada.");
        }

        var isEnrollmentActive = await _enrollmentRepository.EnrollmentIsActiveAsync(input.EnrollmentId);

        if (!isEnrollmentActive)
        {
            throw new NotAllowedException("Não é possível criar um pagamento para uma matrícula inativa.");
        }

        Payment newPaymeny = new Payment
        {
            EnrollmentId = input.EnrollmentId,
            StudentId = enrollment.StudentId,
            Amount = input.Amount,
            Status = PaymentStatus.Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepository.AddAsync(newPaymeny);

        await ClearPaymentCachesAsync();

        return new PaymentOutputDto(
            newPaymeny.PaymentId,
            newPaymeny.EnrollmentId,
            newPaymeny.StudentId,
            newPaymeny.Amount,
            newPaymeny.Status,
            newPaymeny.IsActive,
            newPaymeny.CreatedAt,
            null,
            null
        );
    }

    public async Task<ProcessPaymentOutputDto> ProcessPaymentAsync(string idempotencyKey, ProcessPaymentDto dto)
    {
        var payment = await _paymentRepository.GetByIdAsync(dto.PaymentId);
        if (payment == null)
            throw new NotFoundException("Pagamento não encontrado.");

        var processableSpec = new PaymentProcessableSpecification();
        if (!processableSpec.IsSatisfiedBy(payment))
        {
            throw new NotAllowedException(processableSpec.ErrorMessage);
        }

        var strategy = _strategyFactory.GetStrategy(dto.type);

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            var gatewayResult = await strategy.ProcessAsync(payment, _paymentGateway, idempotencyKey, cts.Token);

            if (!gatewayResult.IsSuccess)
            {
                throw new BadRequestExecption($"Falha ao processar pagamento [{gatewayResult.ErrorCode}]: {gatewayResult.ErrorMessage}");
            }

            payment.Status = PaymentStatus.Paid;
            payment.PaidAt = gatewayResult.ProcessedAt;
            payment.ExternalTransactionId = gatewayResult.TransactionId;
            payment.ReceiptUrl = gatewayResult.ReceiptUrl;

            await _paymentRepository.UpdateAsync(payment);

            await ClearPaymentCachesAsync();

            return new ProcessPaymentOutputDto(true, "Pagamento processado com sucesso.", payment.ExternalTransactionId);
        }
        catch (OperationCanceledException)
        {
            throw new GatewayTimeoutException("A comunicação com o provedor de pagamento excedeu o tempo limite.");
        }
    }

    public async Task<RefundPaymentOutputDto> RefundPaymentAsync(string idempotencyKey, RefundPaymentDto dto)
    {
        var payment = await _paymentRepository.GetByIdAsync(dto.PaymentId);
        if (payment == null)
            throw new NotFoundException("Pagamento não encontrado.");

        if (payment.Status != PaymentStatus.Paid || string.IsNullOrEmpty(payment.ExternalTransactionId))
            throw new NotAllowedException("Apenas pagamentos processados e com ID de transação podem ser estornados.");

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            // Chamada direta ao Adapter de Gateway para estorno
            var gatewayResult = await _paymentGateway.RefundTransactionAsync(
                payment.ExternalTransactionId,
                cts.Token,
                idempotencyKey
            );

            if (!gatewayResult.IsSuccess)
            {
                throw new BadRequestExecption($"Falha ao estornar pagamento [{gatewayResult.ErrorCode}]: {gatewayResult.ErrorMessage}");
            }

            payment.Status = PaymentStatus.Refunded;
            payment.IsActive = false;
            payment.RefundedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);

            await ClearPaymentCachesAsync();

            return new RefundPaymentOutputDto(true, "Estorno realizado com sucesso.");
        }
        catch (OperationCanceledException)
        {
            throw new GatewayTimeoutException("A comunicação com o provedor de pagamento excedeu o tempo limite durante o estorno.");
        }
    }

    protected async Task<bool> ValidateRoleAcess(string targetIdentityUserId)
    {
        var currentUserId = _currentUserService.GetUserId();
        var isAdmin = _currentUserService.IsInRole(UserRole.Admin);

        if (currentUserId != targetIdentityUserId && !isAdmin)
        {
            throw new NotAllowedException("Você não possuí permissão suficiente para acessar registro!");
        }

        return true;
    }

    private async Task ClearPaymentCachesAsync()
    {
        await _cacheService.RemoveByPrefixAsync(PAYMENT_LIST_PREFIX);
        await _cacheService.RemoveByPrefixAsync(PAYMENT_ITEM_PREFIX);
        await _cacheService.RemoveByPrefixAsync(PAYMENT_BY_STUDENT_PREFIX);
        await _cacheService.RemoveByPrefixAsync(PAYMENT_BY_ENROLLMENT_PREFIX);
    }
}
