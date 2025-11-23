using AllinOne.Models.Builders;
using AllinOne.Models.Configuration;
using AllinOne.Models.Requests.UserRequests;
using AllinOne.Models.SqliteDatabase;
using Microsoft.Extensions.Options;

namespace AllinOne.Utils.Mappers
{
    public class UserMapper //: IEntityMapper<User,UserResponse, CreateUserRequest, UpdateUserRequest>
    {
        private readonly IOptions<UserPasswordSection> _passwordOptions;

        public UserMapper(IOptions<UserPasswordSection> passwordOptions)
        {
            _passwordOptions = passwordOptions;
        }
        public User ToEntity(CreateUserRequest request)
        {
            var builder = new UserBuilder(_passwordOptions);
            var user = builder.SetDateOfBirth(request.DateOfBirth)
                        .SetHomeAddress(request.HomeAddress)
                        .SetPassword(request.Password)
                        .SetRole(request.Role)
                        .SetFirstName(request.FirstName)
                        .SetLastName(request.LastName)
                        .SetEmail(request.Email)
                        .SetPhone(request.Phone)
                        .Build();
            return user;
        }

/*        public UserResponse ToResponse(User entity)
        {
            return new UserResponse
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Description = entity.Description
            };
        }

        public void UpdateEntity(UpdateUserRequest request, User entity)
        {
            if (!string.IsNullOrEmpty(request.Description))
            {
                entity.Description = request.Description;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }*/
    }
}
