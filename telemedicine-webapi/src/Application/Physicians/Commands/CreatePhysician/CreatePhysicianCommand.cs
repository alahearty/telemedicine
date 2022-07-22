using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Physicians.Commands.CreatePhysician;

public record CreatePhysicianCommand : IRequest<BaseResponse>
{
    public string? Title { get; init; }
}

public class CreatePhysicianCommandHandler : IRequestHandler<CreatePhysicianCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public CreatePhysicianCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(CreatePhysicianCommand request, CancellationToken cancellationToken)
    {
        var entity = new Physician();

        entity.FirstName = request.Title;

        _context.PhysicianRepository.Add(entity);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful(entity.Id):OperationResult.NotSuccessful("");
    }
}
