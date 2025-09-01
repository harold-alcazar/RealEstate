
namespace RealEstate.Application.DTOs
{
    public record RegisterRequest(string Email, string Password, string? PhoneNumber);
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string AccessToken, string UserId, string Email);
}
