using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Patients.Commands.CreatePatients;

public record CreatePatientCommand : IRequest<BaseResponse>
{
    public string? Title { get; init; }
}

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public CreatePatientCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();

        entity.Title = request.Title;

        _context.TodoListRepository.Add(entity);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful(entity.Id):OperationResult.NotSuccessful("Unable to save Todo list");
    }
}
