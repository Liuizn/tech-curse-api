using System.ComponentModel.DataAnnotations;
using tech_curse_api.src.Domain.Enums;

namespace tech_curse_api.src.Application.DTOs;

public record RegisterInputDto(string Name, string Email, [Required] UserRole Role, string Password, string ConfirmPassword);
public record LoginInputDto(string Email, string Password);
public record RefreshTokenInputDto(string AccessToken, string RefreshToken);
public record TokenOutputDto(string AccessToken, DateTime ExpiresAt);
public record AuthOutputDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);