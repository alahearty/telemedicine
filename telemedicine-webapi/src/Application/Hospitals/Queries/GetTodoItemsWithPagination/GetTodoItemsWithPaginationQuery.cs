using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Application.Common.Models;
using MediatR;
using telemedicine_webapi.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Hospitals.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemDataDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemDataDto>>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetTodoItemsWithPaginationQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemDataDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var todoItems=await _context.TodoItemRepository.GetAllAsync();
            
        return await todoItems.OrderBy(x=>x.Title).AsQueryable<TodoItem>() 
            .ProjectTo<TodoItemDataDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber,request.PageSize);     
    }
}
