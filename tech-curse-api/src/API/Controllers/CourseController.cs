using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;

namespace tech_curse_api.src.API.Controllers
{
    [ApiController]
    [Route("tech-curse/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Post([FromBody] CoursePostDto input)
        {
            var result = await _courseService.CreateAsync(input);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] CoursePaginationParamsDto searchParams)
        {
            var result = await _courseService.GetPagedAsync(searchParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _courseService.GetByIdAsync(id);

            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] CoursePostDto input)
        {
            CoursePutDto dto = new CoursePutDto(id, input.Titulo, input.Descricao, input.Categoria, input.CargaHoraria);

            var result = await _courseService.UpdateAsync(dto);

            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _courseService.DeleteAsync(id);

            return result ? NoContent() : NotFound();
        }
    }
}
