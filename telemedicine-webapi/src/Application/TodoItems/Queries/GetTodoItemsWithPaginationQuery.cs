using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Application.Common.Models;
using MediatR;
using telemedicine_webapi.Domain.Entities;
//using telemedicine_webapi.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace telemedicine_webapi.Application.TodoItems.Queries;

// public record GetTodoItemsWithPaginationQuery : IRequest<BaseResponse>
// {
//     public int ListId { get; init; }
//     public int PageNumber { get; init; } = 1;
//     public int PageSize { get; init; } = 10;
// }

// public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, BaseResponse>
// {
//     private readonly IUnitOfWork _context;
//     private readonly IMapper _mapper;

//     public GetTodoItemsWithPaginationQueryHandler(IUnitOfWork context, IMapper mapper)
//     {
//         _context = context;
//         _mapper = mapper;
//     }

//     public async Task<BaseResponse> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
//     {
//         var todoItems=await _context.TodoItemRepository.GetAllAsync();
//         return await todoItems.OrderBy(x => x.Title)
//             .AsQueryable<TodoItem>()
//             .ProjectTo<TodoItemDataDto>(_mapper.ConfigurationProvider)
//             .PaginatedListAsync(request.PageNumber, request.PageSize);
//     }
//}
