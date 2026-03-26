using Spark.API.DTOs.Auth;

namespace Spark.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto dto);
        Task<AuthResponseDto> Login(LoginDto dto);
    }
}
