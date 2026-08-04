using Microsoft.AspNetCore.Mvc;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Exceptions;


namespace tech_curse_api.src.API.Controllers;

[ApiController]
[Route("tech-curse/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authAppService)
    {
        _authService = authAppService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterInputDto input)
    {
        var actionResult = await _authService.RegisterAsync(input);

        return StatusCode(201, "Usuário registrado com sucesso.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginInputDto input)
    {
        var authResult = await _authService.LoginAsync(input);

        return Ok(authResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenInputDto input)
    {
        var refreshResult = await _authService.RefreshAsync(input);

        return Ok(refreshResult);
    }
}