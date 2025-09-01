using RealEstate.Application.DTOs;
using RealEstate.Application.Results;

namespace RealEstate.Application.IServices
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest req);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest req);
    }
}
