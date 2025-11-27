using AllinOne.Models.Requests.OrdrRequests;
using AllinOne.Models.Requests.PatientRequests;
using AllinOne.Models.Responses;
using AllinOne.Models.SqliteDatabase;
using AllinOne.Repositories.Sqlite.Interface;
using AllinOne.Services.Interfaces;
using AllinOne.Services.Interfaces.ModelHandlingInterfaces;
using AllinOne.Utils.Mappers.Interfaces;

namespace AllinOne.Services.Implementations.ModelHandlingServices
{
    public class PatientHandlingService : IPatientHandlingService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEntityHandlingService<Patient, PatientResponse> _entityService;
        private readonly ILogger<PatientHandlingService> _logger;
        private readonly IEntityMapper<Patient, PatientResponse, CreatePatientRequest, UpdatePatientRequest> _patientMapper;

        public PatientHandlingService(IOrderRepository orderRepository,
            IEntityHandlingService<Patient, PatientResponse> entityService,
            ILogger<PatientHandlingService> logger,
            IEntityMapper<Patient, PatientResponse, CreatePatientRequest, UpdatePatientRequest> patientMapper)
        {
            _orderRepository = orderRepository;
            _entityService = entityService;
            _logger = logger;
            _patientMapper = patientMapper;
        }
    }
}
