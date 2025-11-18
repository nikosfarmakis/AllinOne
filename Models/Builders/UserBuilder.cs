using AllinOne.Constants;
using AllinOne.Models.SqliteDatabase;
using AllinOne.Models.ValueObjects;
using AllinOne.Utils.Extensions;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace AllinOne.Models.Builders
{
    public class UserBuilder
    {
        //private readonly User _user = new();
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private int _age;
        private UserRoles _role;
        private bool _isAdmin = false;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private Address _homeAddress;

        private readonly List<string> _validationErrors = new();

        public UserBuilder SetHomeAddress(Address homeAddress)
        {
            _homeAddress = homeAddress;
            return this;
        }
        public UserBuilder SetFirstName(string name)
        {
            if (!name.ValidateName("First name", out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _firstName = name;
            }
            return this;
        }

        public UserBuilder SetLastName(string name)
        {
            if (!name.ValidateName("Last name", out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _lastName = name;
            }
            return this;
        }

        public UserBuilder SetEmail(string email)
        {
            if (!email.IsValidEmail(out List<string> validationErrors))
            {
                _validationErrors.AddRange(validationErrors);
            }
            else
            {
                _email = email.Trim();
            }
            return this;
        }
        public UserBuilder SetPhone(string phone)
        {
            _phone = phone;
            return this;
        }

        public UserBuilder SetAge(int age)
        {
            if (age < 18)
            {
                _validationErrors.Add("Age cannot be less than 18.");
            }
            else
            {
                _age = age;
            }
            return this;
        }

        public UserBuilder SetRole(UserRoles role)
        {
            //_user.Role = role;
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
                Age = _age,
                Role = _role,
                IsAdmin = _isAdmin,
                Phone = _phone,
                Email = _email,
                HomeAddress = _homeAddress
            };
        }
    }
}
