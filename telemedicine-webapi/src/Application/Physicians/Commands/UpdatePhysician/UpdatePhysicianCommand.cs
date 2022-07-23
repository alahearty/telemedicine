using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Physicians.Commands.UpdatePhysician;

public record UpdatePhysicianCommand : IRequest<BaseResponse>
{
    public int Id { get; init; }

    public string? Email { get; init; }
}

public class UpdatePhysicianCommandHandler : IRequestHandler<UpdatePhysicianCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public UpdatePhysicianCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(UpdatePhysicianCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.PhysicianRepository.GetById(request.Id);
        if (entity == null) return OperationResult.NotSuccessful($"Physician with Id-{request.Id} not found");

        entity.FirstName = request.Email;

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to update physician");
    }
}
