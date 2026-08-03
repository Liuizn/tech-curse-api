using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;

namespace tech_curse_api.src.API.Controllers
{
    [ApiController]
    [Route("tech-curse/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] EnrollmentInputDto input)
        {
            var actionResult = await _enrollmentService.CreateAsync(input);

            return Accepted("Aluno matriculado com sucesso.");
        }
    }
}
