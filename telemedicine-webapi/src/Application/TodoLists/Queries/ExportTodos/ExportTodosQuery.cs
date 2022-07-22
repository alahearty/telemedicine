using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;

public record ExportTodosQuery : IRequest<ExportTodosVm>
{
    public int ListId { get; init; }
}

public class ExportTodosQueryHandler : IRequestHandler<ExportTodosQuery, ExportTodosVm>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;
    private readonly ICsvFileBuilder _fileBuilder;

    public ExportTodosQueryHandler(IUnitOfWork context, IMapper mapper, ICsvFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportTodosVm> Handle(ExportTodosQuery request, CancellationToken cancellationToken)
    {
        var todoItems=await _context.TodoListRepository.GetAllAsync();
        var records=new List<TodoItemRecord>();
        // var records = await _context.TodoItems
        //         .Where(t => t.ListId == request.ListId)
        //         .ProjectTo<TodoItemRecord>(_mapper.ConfigurationProvider)
        //         .ToListAsync(cancellationToken);

        var vm = new ExportTodosVm(
            "TodoItems.csv",
            "text/csv",
            _fileBuilder.BuildTodoItemsFile(records));

        return vm;
    }
}
