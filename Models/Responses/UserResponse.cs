using AllinOne.Constants;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Responses
{
    public class UserResponse : PersonResponse
    {
        public UserRoles Role { get; internal set; }
        public string Password { get; internal set; }
    }
}
