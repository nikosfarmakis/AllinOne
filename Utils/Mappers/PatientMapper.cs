using AllinOne.Builders;
using AllinOne.Models.Requests.PatientRequests;
using AllinOne.Models.Responses;
using AllinOne.Models.SqliteDatabase;
using AllinOne.Utils.Mappers.Interfaces;

namespace AllinOne.Utils.Mappers
{
    public class PatientMapper : IEntityMapper<Patient, PatientResponse, CreatePatientRequest, UpdatePatientRequest>
    {
        public Patient ToEntity(CreatePatientRequest request)
        {
            var builder = new PatientBuilder();
            var user = builder.SetDateOfBirth(request.DateOfBirth)
                        .SetFirstName(request.FirstName)
                        .SetLastName(request.LastName)
                        .SetEmail(request.Email)
                        .SetPhone(request.Phone)
                        .SetAMKA(request.AMKA)
                        .SetNotes(request.Notes)
                        .Build();
            return user;
        }

        public PatientResponse ToResponse(Patient entity)
        {
            return new PatientResponse
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Phone = entity.Phone,
                Email = entity.Email,
                DateOfBirth = entity.DateOfBirth,
                Age = entity.Age,
                AMKA = entity.AMKA,
                Notes = entity.Notes
            };
        }

        public void UpdateEntity(UpdatePatientRequest request, Patient entity)
        {
            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.Phone = request.Phone;
            entity.Email = request.Email;
            entity.DateOfBirth = request.DateOfBirth;
            entity.AMKA = request.AMKA;
            entity.Notes = request.Notes;
        }
    }
}
