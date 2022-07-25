using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Hospitals.Commands.CreateHospital;

public record CreateHospitalCommand : IRequest<BaseResponse>
{
    public string? Name { get; init; }
    public string? Location { get; init; }
}

public class CreateHospitalCommandHandler : IRequestHandler<CreateHospitalCommand, BaseResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateHospitalCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> Handle(CreateHospitalCommand request, CancellationToken cancellationToken)
    {
        var entity = new Hospital();

        entity.HospitalName = request.Name;
        entity.Address = request.Location;

        _unitOfWork.HospitalRepository.Add(entity);

        var commitResult=await _unitOfWork.SaveChangesAsync(cancellationToken);
        if(commitResult.WasSuccesful) return OperationResult.Successful(entity.Id);
        return OperationResult.NotSuccessful("Unable to save hospital");
    }
}
