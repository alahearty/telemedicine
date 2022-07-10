using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Security;
using telemedicine_webapi.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Hospitals.Queries.GetTodos;

[Authorize]
public record GetHospitalsVmsQuery : IRequest<HospitalsVm>;

public class GetHospitalsQueryHandler : IRequestHandler<GetHospitalsVmsQuery, HospitalsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetHospitalsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<HospitalsVm> Handle(GetHospitalsVmsQuery request, CancellationToken cancellationToken)
    {
        return new HospitalsVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),

            Lists = await _context.TodoLists
                .AsNoTracking()
                .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken)
        };
    }
}
