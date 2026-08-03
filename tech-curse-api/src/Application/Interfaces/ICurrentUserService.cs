using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.Application.Interfaces;

public interface ICurrentUserService
{
    string? GetUserId();
    string? GetUserEmail();
    bool IsInRole(UserRole roleName);
}
