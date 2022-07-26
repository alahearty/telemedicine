using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments;
public record RespondToScheduledAppointmentCommand(int AppointmentId):IRequest<BaseResponse>;

public class RespondToScheduledAppointmentCommandHandler : IRequestHandler<RespondToScheduledAppointmentCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public RespondToScheduledAppointmentCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }
    public async Task<BaseResponse> Handle(RespondToScheduledAppointmentCommand request, CancellationToken cancellationToken)
    {
        var scheduledAppointment = await _context.AppointmentRepository.GetByIdAsync(request.AppointmentId);
        if (scheduledAppointment == null) return OperationResult.NotSuccessful("Appointment was not scheduled");
        scheduledAppointment.Status = ScheduleStatus.Active;
        //doctor initiate interacton and examine the patient
        //create health analysis report
        //make comment
        //close the schedule
        //update/delete the appointment;
        return OperationResult.Successful();
    }
}

