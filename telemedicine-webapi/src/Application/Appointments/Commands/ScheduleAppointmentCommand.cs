using MediatR;
using Microsoft.Extensions.Caching.Memory;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.ScheduleAppointment;
public record ScheduleAppointmentCommand(int serviceId,string paymentToken):IRequest<BaseResponse>;

public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ICurrentUserService _user;

    public ScheduleAppointmentCommandHandler(IUnitOfWork context, IMemoryCache memoryCache, ICurrentUserService user)
    {
        _context = context;
        _memoryCache = memoryCache;
        _user = user;
    }


    //provide the telemedicine service
    //provide telemedicine payment token
    //confirm payment and save/cached appointment

    public async Task<BaseResponse> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var teleMedService = await _context.TelemedicineServiceRepository.GetByIdAsync(request.serviceId);
        if (teleMedService == null) return OperationResult.NotSuccessful("Telemedicine service not found");

        var isValidPaymentToken = ValidatePaymentToken(request.paymentToken);
        if (!isValidPaymentToken) return OperationResult.NotSuccessful("Payment token is invalid");

        _memoryCache.Remove("paymentToken");

        var patient = await _context.PatientRepository.FirstOrDefaultAsync(user => user.Email == _user.Email!);

        var appointment = new Appointment
        {
            Created = DateTime.Now,
            CreatedBy = null,
            Patient = patient,
            Status = ScheduleStatus.NotAttended,
            TelemedicineService = teleMedService,
        };

        _context.AppointmentRepository.Add(appointment);

        var commitResult = await _context.SaveChangesAsync();
        return commitResult.WasSuccesful ? OperationResult.Successful(appointment.Id) 
            : OperationResult.NotSuccessful("Unable to schedule appointment");
    }

    private bool ValidatePaymentToken(string paymentToken)
    {
        var storedToken=_memoryCache.Get(paymentToken);
        return (storedToken.ToString() != paymentToken) ? false : true;
    }
}
