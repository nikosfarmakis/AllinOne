using AllinOne.Models.SqliteDatabase;
using AllinOne.Utils.Extensions;

namespace AllinOne.Builders
{
    public class PatientBuilder : PersonBuilder<PatientBuilder>
    {
        private string _notes { get; set; }
        private string _AMKA { get; set; }
        public PatientBuilder SetNotes(string notes)
        {
            _notes = notes?.Trim();
            return this;
        }
        public PatientBuilder SetAMKA(string AMKA)
        {
            if (!AMKA.IsValidAMKA(out string error, _dateOfBirth))
            {
                _validationErrors.Add(error);
            }
            else
            {
                _AMKA = AMKA;
            }
            return this;
        }

        public Patient Build()
        {
            if (_validationErrors.Count > 0)
            {
                throw new ArgumentException("Cannot build Patient. Validation errors:\n" + string.Join("\n", _validationErrors));
            }

            return new Patient
            {
                FirstName = _firstName,
                LastName = _lastName,
                DateOfBirth = _dateOfBirth,
                Phone = _phone,
                Email = _email,
                Notes = _notes,
                AMKA = _AMKA
            };
        }
    }
}
