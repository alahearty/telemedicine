using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Security;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Physicians.Commands.PurgePhysician;

// [Authorize(Roles = "Administrator")]
// [Authorize(Policy = "CanPurge")]
public record PurgePhysicianCommand : IRequest<BaseResponse>;

public class PurgePhysicianCommandHandler : IRequestHandler<PurgePhysicianCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public PurgePhysicianCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(PurgePhysicianCommand request, CancellationToken cancellationToken)
    {
        var physicians=await _context.PhysicianRepository.GetAllAsync();
        _context.PhysicianRepository.DeleteMany(physicians);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to purge physicians");
    }
}
