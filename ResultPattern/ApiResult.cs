using AllinOne.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Extensions;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AllinOne.ResultPattern
{
    public class ApiResult<T> /*where T : class?*/
    {
        [JsonPropertyName("status")]
        public bool Status => Error is null;

        [JsonPropertyName("error")]
        public ApiResultError? Error { get; init; }
        [JsonPropertyName("description")]
        public string? Description { get; init; }
        [JsonPropertyName("data")]
        public T? Data { get; init; }
        [JsonIgnore]
        public bool HasException { get; set; }

        public static ApiResult<T> Success(T? data, string? description = null)
        {
            return new ApiResult<T>(data, description);
        }

        public static ApiResult<T> Failure(T? data, ApiResultError? error, bool hasException= false)
        {
            return new ApiResult<T>(data, error, hasException);
        }

        ApiResult(T data, string? description)
        {
            Data = data;
            Description = description;
        }

        ApiResult(T data, ApiResultError? error, bool hasException)
        {
            Data = data;
            Error = error;
            HasException = hasException;
        }
    }

    public class ApiResultError
    {
        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; init; }

        [JsonPropertyName("errors")]
        public object[]? ValidationErrors { get; init; }

        public ApiResultError(string? description = null, object[]? validationErrors = null)
        {
            ErrorDescription = description;
            ValidationErrors = validationErrors;
        }

        public static ApiResultError NotFound()
        {
            return new ApiResultError(ProjectErrorCodes.NotExisting.GetDisplayName());
        }        
        public static ApiResultError GenericFailure(string? message = null)
        {
            return new ApiResultError(string.IsNullOrWhiteSpace(message)? ProjectErrorCodes.InternalError.GetDisplayName(): message);
        }

    }
}
