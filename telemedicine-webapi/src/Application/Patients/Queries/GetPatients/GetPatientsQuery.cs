using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Patients.Queries.GetPatients;

//[Authorize]
public record GetPatientsQuery : IRequest<BaseResponse>;

public class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetPatientsQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients=await _context.PatientRepository.GetAllAsync();
        var patientDtos = _mapper.Map<List<PatientDto>>(patients);
        return OperationResult.Successful(patientDtos);
    }
}
