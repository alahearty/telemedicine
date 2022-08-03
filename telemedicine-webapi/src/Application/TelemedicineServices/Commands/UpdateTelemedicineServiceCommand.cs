using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TelemedicineServices.Commands;
public record UpdateTelemedicineServiceCommand(int id,double cost, string description):IRequest<BaseResponse>;

public class UpdateTelemedicineServiceCommandHandler : IRequestHandler<UpdateTelemedicineServiceCommand, BaseResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateTelemedicineServiceCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }
    public async Task<BaseResponse> Handle(UpdateTelemedicineServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.TelemedicineServiceRepository.GetByIdAsync(request.id);
        if (service == null) return OperationResult.NotSuccessful("Service not found");

        if (request.cost == 0) service.Cost = (decimal)request.cost;
        if (request.description == null) service.Description = request.description;
        service.LastModified = DateTime.UtcNow;
        service.LastModifiedBy = _currentUser.Email;

        _unitOfWork.TelemedicineServiceRepository.Update(service);
        var commitResult = await _unitOfWork.SaveChangesAsync();

        return commitResult.WasSuccesful ? OperationResult.Successful() 
            : OperationResult.NotSuccessful("Unable to update service");
    }
}

