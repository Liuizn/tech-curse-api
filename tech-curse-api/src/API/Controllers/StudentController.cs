using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.API.Controllers
{
    [ApiController]
    [Route("tech-curse/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] StudentPostDto input)
        {
            var result = await _studentService.CreateAsync(input);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery]PaginationParamsDto searchParams)
        {
            var result = await _studentService.GetPagedAsync(searchParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _studentService.GetByIdAsync(id);

            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] StudentPutDto input)
        {
            StudentPutDto dto = new StudentPutDto(id, input.Nome);

            var result = await _studentService.UpdateAsync(dto);

            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _studentService.DeleteAsync(id);

            return result ? NoContent() : NotFound();
        }
    }
}
