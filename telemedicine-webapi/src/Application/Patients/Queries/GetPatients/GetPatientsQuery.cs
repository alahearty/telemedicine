using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Patients.Queries.GetPatients;

//[Authorize]
public record GetPatientsQuery : IRequest<PatientFileRecordsVm>;

public class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, PatientFileRecordsVm>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetPatientsQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PatientFileRecordsVm> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients=await _context.PatientRepository.GetAllAsync();
        return new PatientFileRecordsVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),

             Lists = patients.AsQueryable<Patient>()
                .ProjectTo<PatientFileRecordListDto>(_mapper.ConfigurationProvider)
                .ToList()
        };
    }
}
