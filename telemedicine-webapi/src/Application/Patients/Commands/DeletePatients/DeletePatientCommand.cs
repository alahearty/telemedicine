using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Patients.Commands.DeletePatients;

public record DeletePatientCommand(int Id) : IRequest<BaseResponse>;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public DeletePatientCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var entity =  await _context.TodoListRepository
            .FirstOrDefaultAsync(l => l.Id == request.Id);
            
        if (entity == null)return OperationResult.NotSuccessful($"Patient with Id-{request.Id} not found");
        _context.TodoListRepository.Delete(entity);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to delete patient");
    }
}
