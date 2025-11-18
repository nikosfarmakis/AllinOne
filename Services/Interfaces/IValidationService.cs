using AllinOne.Models.Configuration;
using AllinOne.Models.Requests.Authorization;
using AllinOne.ResultPattern;

namespace AllinOne.Services.Interfaces
{
    public interface IValidationService
    {
        //bool IsApiKeyValid(string apiKey);
        //UserWithAccess? GetUserByApiKey(string apiKey);
        Task<ApiResult<string?>> ValidUser(LoginRequest request, string apiKey);

    }
}
