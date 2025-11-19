using AllinOne.Constants;
using AllinOne.Models.SqliteDatabase;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace AllinOne.Models.Builders
{
    public class UserBuilder : PersonBuilder
    {   
        private UserRoles _role;
        private bool _isAdmin = false;

        public UserBuilder SetRole(UserRoles role)
        {
            if (role == UserRoles.Admin)
            {
                _isAdmin = true;
            }
            _role = role;
            return this;
        }
        public User Build()
        {
            if (_validationErrors.Count > 0)
            {
                throw new ArgumentException("Cannot build User. Validation errors:\n" + string.Join("\n", _validationErrors));
            }

            return new User
            {
                FirstName = _firstName,
                LastName = _lastName,
                DateOfBirth = _dateOfBirth,
                Phone = _phone,
                Email = _email,
                HomeAddress = _homeAddress,
                Role = _role,
                IsAdmin = _isAdmin
            };
        }
    }
}
