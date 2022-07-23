using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Enums;
using MediatR;
//using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Hospitals.Queries.GetHospitals;

//[Authorize]
public record GetHospitalsVmsQuery : IRequest<HospitalsVm>;

public class GetHospitalsQueryHandler : IRequestHandler<GetHospitalsVmsQuery, HospitalsVm>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetHospitalsQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<HospitalsVm> Handle(GetHospitalsVmsQuery request, CancellationToken cancellationToken)
    {
        var hospitals = await _context.HospitalRepository.GetAllAsync();

        return new HospitalsVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),

            Lists = hospitals.AsQueryable<Hospital>()
                .ProjectTo<HospitalDto>(_mapper.ConfigurationProvider)
                .ToList()
        };
    }
}
