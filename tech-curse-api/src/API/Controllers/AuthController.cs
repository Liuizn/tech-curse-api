using Microsoft.AspNetCore.Mvc;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;


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
        if (input.Password != input.ConfirmPassword)
            return BadRequest("As senhas não coincidem.");

        var actionResult = await _authService.RegisterAsync(input);

        if (!actionResult.status)
        {
            return BadRequest(new { message = "Erro ao registrar usuário.", errors = actionResult.JSON });
        }

        return StatusCode(201, "Usuário registrado com sucesso.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginInputDto input)
    {
        var authResult = await _authService.LoginAsync(input);
        if (authResult == null) return Unauthorized("E-mail ou senha incorretos.");

        return Ok(authResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenInputDto input)
    {
        var refreshResult = await _authService.RefreshAsync(input);
        if (refreshResult == null) return Unauthorized("Refresh Token inválido ou expirado.");

        return Ok(refreshResult);
    }
}