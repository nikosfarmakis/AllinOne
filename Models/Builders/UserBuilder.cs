using AllinOne.Constants;
using AllinOne.Models.Configuration;
using AllinOne.Models.SqliteDatabase;
using AllinOne.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace AllinOne.Models.Builders
{
    public class UserBuilder : PersonBuilder
    {   
        private UserRoles _role;
        private bool _isAdmin = false;
        private string _password;
        private readonly UserPasswordSection _uPassSection;


        public UserBuilder(IOptions<UserPasswordSection> userPasswordSection) 
        {
            _uPassSection = userPasswordSection.Value;
        }

        public UserBuilder SetPassword(string password)
        {
            if (!password.IsValidPassword(out string error, _uPassSection.MinLength, _uPassSection.MaxLength,
                _uPassSection.RequireLettersChar, _uPassSection.RequireNumbers, _uPassSection.RequireSpecialChar))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _password = password;
            }
            return this;
        }

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
                IsAdmin = _isAdmin,
                Password =_password
            };
        }
    }
}
