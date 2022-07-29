using System.Text;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.TeleMedicinePayment;
public record MakePaymentCommand(int telemedicineServiceId, decimal amountPaid):IRequest<BaseResponse>;

public class MakePaymentCommandHandler : IRequestHandler<MakePaymentCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMemoryCache _memoryCache;

    public MakePaymentCommandHandler(IUnitOfWork context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
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

        string paymentToken = GeneratePaymentToken(16);
        _memoryCache.Set("paymentToken", paymentToken);

        _context.TelemedicinePaymentRepository.Add(telServicePayment);
        var result = await _context.SaveChangesAsync();

        return result.WasSuccesful ? OperationResult.Successful(new {paymentToken, telServicePayment.Id })
            : OperationResult.NotSuccessful("Unable to save");
    }

    private string GeneratePaymentToken(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var token = new StringBuilder();

        for(int i = 0; i < length; i++)
        {
            token.Append(chars[random.Next(chars.Length)]);
        }
        return token.ToString();
    }
}

