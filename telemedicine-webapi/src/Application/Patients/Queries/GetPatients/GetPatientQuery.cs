using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Patients.Queries.GetPatients;

public record GetPatientQuery(int id):IRequest<BaseResponse>;

public class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetPatientQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BaseResponse> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        var patient = await _context.PatientRepository.GetByIdAsync(request.id);
        var patientDto = _mapper.Map<PatientDto>(patient);

        return OperationResult.Successful(patientDto);
    }
}
