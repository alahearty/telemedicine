using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.TeleMedicinePayment;
public record MakePaymentCommand(int telemedicineServiceId, decimal amountPaid):IRequest<BaseResponse>;

public class MakePaymentCommandHandler : IRequestHandler<MakePaymentCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public MakePaymentCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(MakePaymentCommand request, CancellationToken cancellationToken)
    {
        var telMedService = await _context.TelemedicineServiceRepository.GetByIdAsync(request.telemedicineServiceId);
        if (telMedService == null) return OperationResult.NotSuccessful("Telemedicine service not found");
        if(request.amountPaid < telMedService.Cost) return OperationResult.NotSuccessful("Insufficient amount paid for service");

        var telServicePayment = new TelemedicinePayment
        {
            AmountPaid = request.amountPaid,
            IsPaid = true,
            Created = DateTime.UtcNow,
            CreatedBy = null,
            Service = telMedService
        };

        _context.TelemedicinePaymentRepository.Add(telServicePayment);
        var result = await _context.SaveChangesAsync();

        return result.WasSuccesful ? OperationResult.Successful(telServicePayment.Id)
            : OperationResult.NotSuccessful("Unable to save");

    }
}

