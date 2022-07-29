using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TelemedicineServices.Commands;
public record DeleteTelemedicineServiceCommand(int id):IRequest<BaseResponse>;

public class DeleteTelemedicineServiceCommandHandler : IRequestHandler<DeleteTelemedicineServiceCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public DeleteTelemedicineServiceCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }
    public async Task<BaseResponse> Handle(DeleteTelemedicineServiceCommand request, CancellationToken cancellationToken)
    {
        var telMedService = await _context.TelemedicineServiceRepository.GetByIdAsync(request.id);
        if (telMedService == null) return OperationResult.NotSuccessful("Service not found");

        _context.TelemedicineServiceRepository.Delete(telMedService);
        var commitStatus = await _context.SaveChangesAsync();

        return commitStatus.WasSuccesful ? OperationResult.Successful()
            : OperationResult.NotSuccessful("Unable to delete service");
    }
}
