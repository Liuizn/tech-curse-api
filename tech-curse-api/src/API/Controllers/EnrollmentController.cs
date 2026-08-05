using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;

namespace tech_curse_api.src.API.Controllers
{
    [ApiController]
    [Route("tech-curse/[controller]")]
    [Tags("Enrollments")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Realiza a matrícula de um aluno em um curso.",
            Description = "**Acesso:** Requer usuário autenticado."
        )]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Aluno matriculado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Aluno ou Curso não encontrados.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Aluno já está matriculado neste curso.", typeof(ProblemDetails))]
        public async Task<IActionResult> Post([FromBody] EnrollmentInputDto input)
        {
            var actionResult = await _enrollmentService.CreateAsync(input);

            return Accepted("Aluno matriculado com sucesso.");
        }
    }
}
