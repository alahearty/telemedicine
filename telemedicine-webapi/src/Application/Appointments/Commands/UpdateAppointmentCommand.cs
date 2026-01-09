using MediatR;
using System.ComponentModel.DataAnnotations;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Appointments.Commands;

public record UpdateAppointmentCommand : IRequest<BaseResponse>
{
    [Required]
    public int Id { get; set; }
    
    public DateTime? AppointmentDate { get; set; }
    
    public string? Notes { get; set; }
    
    public ScheduleStatus? Status { get; set; }
}

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateAppointmentCommandHandler(IUnitOfWork context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.AppointmentRepository.GetByIdAsync(request.Id);
        
        if (appointment == null)
        {
            return OperationResult.NotSuccessful($"Appointment with ID {request.Id} not found");
        }

        if (request.AppointmentDate.HasValue)
        {
            appointment.AppointmentDate = request.AppointmentDate.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            appointment.Notes = request.Notes;
        }

        if (request.Status.HasValue)
        {
            appointment.Status = request.Status.Value;
        }

        appointment.LastModified = DateTime.UtcNow;
        appointment.LastModifiedBy = _currentUser.Email;

        _context.AppointmentRepository.Update(appointment);
        var commitResult = await _context.SaveChangesAsync();

        return commitResult.WasSuccesful 
            ? OperationResult.Successful(appointment)
            : OperationResult.NotSuccessful("Unable to update appointment");
    }
}

