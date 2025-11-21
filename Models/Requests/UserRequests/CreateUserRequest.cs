using AllinOne.Constants;
using AllinOne.Models.Requests.PersonRequests;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Requests.UserRequests
{
    public class CreateUserRequest : CreatePersonRequest
    {
        [Required]
        public UserRoles Role { get; internal set; }
        [Required]
        public string Password { get; internal set; }
    }
}