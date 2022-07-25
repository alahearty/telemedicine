using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Hospitals.Commands.UpdateHospital;

public record UpdateHospitalCommand : IRequest<BaseResponse>
{
    public int Id { get; init; }

    public string? HospitalName { get; init; }
}

public class UpdateHospitalCommandHandler : IRequestHandler<UpdateHospitalCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public UpdateHospitalCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(UpdateHospitalCommand request, CancellationToken cancellationToken)
    {
        var entity =  await _context.HospitalRepository.GetByIdAsync(request.Id);

        if (entity == null) return OperationResult.NotSuccessful($"Hospital with Id-{request.Id} not found");

        entity.HospitalName = request.HospitalName;

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful(entity.Id):OperationResult.NotSuccessful("Unable to update hospital");
    }
}
