using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Physicians.Commands.DeletePhysician;

public record DeletePhysicianCommand(int Id) : IRequest<BaseResponse>;

public class DeletePhysicianCommandHandler : IRequestHandler<DeletePhysicianCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public DeletePhysicianCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(DeletePhysicianCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PhysicianRepository.GetByIdAsync(request.Id);

        if (entity == null) return OperationResult.NotSuccessful($"Physician with Id-{request.Id} not found");
        _context.PhysicianRepository.Delete(entity);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to delete physician");
    }
}
