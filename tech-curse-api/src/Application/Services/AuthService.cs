using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;

namespace tech_curse_api.src.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<bool> RegisterAsync(RegisterInputDto input)
    {
        var user = new IdentityUser { UserName = input.Email, Email = input.Email };
        var result = await _userManager.CreateAsync(user, input.Password);

        if (result.Succeeded)
        {
            // Vincula o novo usuário ao papel padrão de "Student"
            await _userManager.AddToRoleAsync(user, "Student");
            return true;
        }

        return false;
    }

    public async Task<AuthOutputDto?> LoginAsync(LoginInputDto input)
    {
        var user = await _userManager.FindByEmailAsync(input.Email);
        if (user == null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: false);
        if (!result.Succeeded) return null;

        // Busca os papéis (roles) do usuário para embutir no Token JWT
        var roles = await _userManager.GetRolesAsync(user);

        // Gera o token JWT através do serviço especializado
        var tokenProcess = _tokenService.GenerateJwtToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(user, "JWTApp", "RefreshToken", refreshToken);

        return new AuthOutputDto(tokenProcess.AccessToken, refreshToken, tokenProcess.ExpiresAt);
    }

    public async Task<AuthOutputDto?> RefreshAsync(RefreshTokenInputDto input)
    {
        // 1. Extrai o usuário a partir do Access Token expirado
        var principal = _tokenService.GetPrincipalFromExpiredToken(input.AccessToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null) return null;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        // 2. Recupera o Refresh Token salvo no banco do Identity
        var savedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "JWTApp", "RefreshToken");

        // 3. Valida se o Refresh Token enviado bate com o do banco
        if (savedRefreshToken != input.RefreshToken) return null;

        // 4. Se for válido, gera um novo par de tokens (Rotação de Refresh Token)
        var roles = await _userManager.GetRolesAsync(user);

        // Gera o token JWT através do serviço especializado
        var tokenProcess = _tokenService.GenerateJwtToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(user, "JWTApp", "RefreshToken", refreshToken);

        return new AuthOutputDto(tokenProcess.AccessToken, refreshToken, tokenProcess.ExpiresAt);
    }
}
