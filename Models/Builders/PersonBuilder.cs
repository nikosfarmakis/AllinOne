using AllinOne.Models.SqliteDatabase;
using AllinOne.Models.SqliteDatabase.ValueObjects;
using AllinOne.Utils.Extensions;

namespace AllinOne.Models.Builders
{
    public abstract class PersonBuilder
    {
        protected string _firstName = string.Empty;
        protected string _lastName = string.Empty;
        protected string _phone = string.Empty;
        protected string _email = string.Empty;
        protected DateTime _dateOfBirth;
        protected Address _homeAddress;

        protected readonly List<string> _validationErrors = new();

        public PersonBuilder SetDateOfBirth(DateTime dateOfBirth)
        {
            if (!dateOfBirth.ValidDateOfBirth(out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _dateOfBirth = dateOfBirth;
            }
            return this;
        }
        public PersonBuilder SetHomeAddress(Address homeAddress)
        {
            //TODO Address VALIDATOR
            if (homeAddress == null)
            {
                _validationErrors.Add("Patient home address  cannot be null.");
            }
            else
            {
                _homeAddress = homeAddress;
            }
            return this;
        }
        public PersonBuilder SetFirstName(string name)
        {
            if (!name.IsValidateName("First name", out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _firstName = name;
            }

            return this;
        }

        public PersonBuilder SetLastName(string name)
        {
            if (!name.IsValidateName("Last name", out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _lastName = name;
            }

            return this;
        }

        public PersonBuilder SetEmail(string email)
        {
            if (!email.IsValidEmail(out List<string> validationErrors))
            {
                _validationErrors.AddRange(validationErrors);
            }
            else
            {
                _email = email;
            }
            return this;
        }
        public PersonBuilder SetPhone(string phone)
        {
            if (!phone.IsValidPhone(out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _phone = phone;
            }
            return this;
        }
    }
}
