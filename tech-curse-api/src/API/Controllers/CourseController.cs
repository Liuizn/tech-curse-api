using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using MediatR;
using tech_curse_api.src.Application.Features.Courses.Commands.CreateCourse;
using tech_curse_api.src.Application.Features.Courses.Queries.GetCourses;

namespace tech_curse_api.src.API.Controllers;

[ApiController]
[Route("tech-curse/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[Tags("Courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IMediator _mediator;

    public CourseController(ICourseService courseService, IMediator mediator)
    {
        _courseService = courseService;
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    [SwaggerOperation(
        Summary = "Cria um novo curso.",
        Description = "**Acesso:** Requer role de Admin ou Instructor."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Curso criado com sucesso.", typeof(CourseOutputDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Erro de validação.", typeof(ProblemDetails))]
    public async Task<IActionResult> Post([FromBody] CreateCourseCommand command)
    {
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation(
        Summary = "Retorna uma lista paginada de cursos.",
        Description = "**Acesso:** Requer autenticação."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Lista de cursos retornada com sucesso.", typeof(PagedResultDto<CourseOutputDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    public async Task<IActionResult> GetAll([FromQuery] CoursePaginationParamsDto searchParams)
    {
        var result = await _mediator.Send(new GetCoursesQuery(searchParams));
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Retorna os detalhes de um curso específico.",
        Description = "**Acesso:** Requer autenticação."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Detalhes do curso retornados com sucesso.", typeof(CourseOutputDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Curso não encontrado.", typeof(ProblemDetails))]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _courseService.GetByIdAsync(id);

        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Atualiza os dados de um curso existente.",
        Description = "**Acesso:** Requer role de Admin."
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Curso atualizado com sucesso.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Curso não encontrado.", typeof(ProblemDetails))]
    public async Task<IActionResult> Put(int id, [FromBody] CoursePostDto input)
    {
        CoursePutDto dto = new CoursePutDto(id, input.Titulo, input.Descricao, input.Categoria, input.CargaHoraria);

        var result = await _courseService.UpdateAsync(dto);

        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Exclui um curso existente.",
        Description = "**Acesso:** Requer role de Admin."
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Curso excluído com sucesso.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Acesso negado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Curso não encontrado.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Conflito. O curso possui matrículas ativas.", typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _courseService.DeleteAsync(id);

        return result ? NoContent() : NotFound();
    }
}
