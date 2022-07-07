using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Security;
using telemedicine_webapi.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Patients.Queries.GetTodos;

[Authorize]
public record GetTodosQuery : IRequest<PatientFileRecordsVm>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, PatientFileRecordsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PatientFileRecordsVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return new PatientFileRecordsVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),

            Lists = await _context.TodoLists
                .AsNoTracking()
                .ProjectTo<PatientFileRecordListDto>(_mapper.ConfigurationProvider)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken)
        };
    }
}
