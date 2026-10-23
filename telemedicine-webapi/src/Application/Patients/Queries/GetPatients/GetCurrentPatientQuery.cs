using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Patients.Queries.GetPatients;

public record GetCurrentPatientQuery : IRequest<BaseResponse>;

public class GetCurrentPatientQueryHandler : IRequestHandler<GetCurrentPatientQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentPatientQueryHandler(IUnitOfWork context, IMapper mapper, ICurrentUserService currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(GetCurrentPatientQuery request, CancellationToken cancellationToken)
    {
        var patients = await _context.PatientRepository.FindAsync(p => p.Email == _currentUser.Email);
        var patient = patients.FirstOrDefault();
        if (patient == null)
            return OperationResult.NotSuccessful("Patient record not found. Please complete your profile in Update Record.");

        var patientDto = _mapper.Map<PatientDto>(patient);
        return OperationResult.Successful(patientDto);
    }
}
