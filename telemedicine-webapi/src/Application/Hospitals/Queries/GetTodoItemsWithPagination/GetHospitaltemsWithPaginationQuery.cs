using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Application.Common.Models;
using MediatR;
using telemedicine_webapi.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace telemedicine_webapi.Application.Hospitals.Queries.GetTodoItemsWithPagination;

public record GetHospitaltemsWithPaginationQuery : IRequest<PaginatedList<HospitalDataDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetHospitaltemsWithPaginationQuery, PaginatedList<HospitalDataDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<HospitalDataDto>> Handle(GetHospitaltemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.TodoItems
            .Where(x => x.ListId == request.ListId)
            .OrderBy(x => x.Title)
            .ProjectTo<HospitalDataDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
