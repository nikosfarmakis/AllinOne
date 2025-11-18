using System.Security.Claims;

namespace AllinOne.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string username, string appKey);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
