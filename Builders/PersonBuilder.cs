using AllinOne.Models.SqliteDatabase;
using AllinOne.Utils.Extensions;
using Google.Protobuf.WellKnownTypes;

namespace AllinOne.Builders
{
    //recursive generic constraint
    //so that the base builder can return the correct type
    //without this [where TBuilder : PersonBuilder<TBuilder>]the compiler will return PersonBuilder not UserBuilder
    public abstract class PersonBuilder<TBuilder> where TBuilder : PersonBuilder<TBuilder> 
    {
        protected string _firstName = string.Empty;
        protected string _lastName = string.Empty;
        protected string _phone = string.Empty;
        protected string _email = string.Empty;
        protected DateTime _dateOfBirth;

        protected readonly List<string> _validationErrors = new();

        public TBuilder SetDateOfBirth(DateTime? dateOfBirth)
        {
            if(dateOfBirth == null)
            {
                return (TBuilder)this;
            }

            if (!dateOfBirth.Value.ValidDateOfBirth(out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _dateOfBirth = dateOfBirth.Value;
            }
            return (TBuilder)this;
        }

        public TBuilder SetFirstName(string name)
        {
            if (!name.IsValidateName("First name", out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _firstName = name;
            }

            return (TBuilder)this;
        }

        public TBuilder SetLastName(string name)
        {
            if (!name.IsValidateName("Last name", out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _lastName = name;
            }

            return (TBuilder)this;
        }

        public TBuilder SetEmail(string? email)
        {
            if (email.IsNullOrEmptyOrWhitespace())
            {
                return (TBuilder)this;
            }

            if (!email.IsValidEmail(out List<string> validationErrors))
            {
                _validationErrors.AddRange(validationErrors);
            }
            else
            {
                _email = email;
            }
            return (TBuilder)this;
        }
        public TBuilder SetPhone(string? phone)
        {
            if (phone.IsNullOrEmptyOrWhitespace())
            {
                return (TBuilder)this;
            }

            if (!phone.IsValidPhone(out string error))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _phone = phone;
            }
            return (TBuilder)this;
        }
    }
}
