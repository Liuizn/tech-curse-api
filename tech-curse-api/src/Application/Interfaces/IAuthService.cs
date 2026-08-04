using tech_curse_api.src.Application.DTOs;

namespace tech_curse_api.src.Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterInputDto input);
    Task<AuthOutputDto?> LoginAsync(LoginInputDto input);
    Task<AuthOutputDto?> RefreshAsync(RefreshTokenInputDto input);
}
