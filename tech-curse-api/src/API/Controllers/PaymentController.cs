using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;

namespace tech_curse_api.src.API.Controllers;

[ApiController]
[Route("tech-curse/[controller]")]
[Tags("Payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Lista os pagamentos cadastrados (paginado).",
        Description = "**Acesso:** Requer role de Admin ou Instructor."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Retorna a lista paginada.", typeof(PagedResultDto<PaymentOutputDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParamsDto searchParams)
    {
        var result = await _paymentService.GetPagedAsync(searchParams);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Busca os detalhes de um pagamento específico pelo ID.",
        Description = "**Acesso:** Requer usuário autenticado."
    )]  
    [SwaggerResponse(StatusCodes.Status200OK, "Pagamento encontrado.", typeof(PaymentOutputDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pagamento não encontrado.", typeof(ProblemDetails))]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _paymentService.GetByIdAsync(id);
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("student/{studentId}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Lista todos os pagamentos de um estudante.",
        Description = "**Acesso:** Requer role de Admin ou o próprio estudante."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Retorna a lista paginada de pagamentos do estudante.", typeof(PagedResultDto<PaymentOutputDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Estudante não encontrado.", typeof(ProblemDetails))]
    public async Task<IActionResult> GetByStudent(int studentId, [FromQuery] PaginationParamsDto searchParams)
    {
        var result = await _paymentService.GetByStudentIdAsync(studentId, searchParams);

        return Ok(result);
    }

    [HttpGet("enrollment/{enrollmentId}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Lista os pagamentos de uma matrícula.",
        Description = "**Acesso:** Requer role de Admin."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Retorna a lista de pagamentos da matrícula.", typeof(IEnumerable<PaymentOutputDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Matrícula não encontrada.", typeof(ProblemDetails))]
    public async Task<IActionResult> GetByEnrollment(int enrollmentId)
    {
        var result = await _paymentService.GetByEnrollmentIdAsync(enrollmentId);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Cria um novo pagamento no sistema.",
        Description = "**Acesso:** Requer role de Admin."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Pagamento criado com sucesso.", typeof(PaymentOutputDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Erro de validação.", typeof(ProblemDetails))]
    public async Task<IActionResult> Post([FromBody] PaymentPostDto input)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{id}/process")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Processa um pagamento (marca como pago).",
        Description = "**Acesso:** Requer role de Admin"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Pagamento processado com sucesso.", typeof(PaymentOutputDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pagamento não encontrado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Erro ao processar pagamento.", typeof(ProblemDetails))]
    public async Task<IActionResult> Process(int id, [FromBody] ProcessPaymentDto input)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{id}/refund")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Estorna um pagamento (marca como reembolsado).",
        Description = "**Acesso:** Requer role de Admin"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Pagamento estornado com sucesso.", typeof(PaymentOutputDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pagamento não encontrado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Erro ao estornar pagamento.", typeof(ProblemDetails))]
    public async Task<IActionResult> Refund(int id, [FromBody] RefundPaymentDto input)
    {
        throw new NotImplementedException();
    }
}
