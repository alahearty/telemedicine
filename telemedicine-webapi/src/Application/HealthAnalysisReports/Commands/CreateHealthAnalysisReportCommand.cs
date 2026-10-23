using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.HealthAnalysisReports.Commands;

public record CreateHealthAnalysisReportCommand : IRequest<BaseResponse>
{
    public int Patientid { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PhysicianId { get; set; }
}

public class CreateHealthAnalysisReportCommandHandler : IRequestHandler<CreateHealthAnalysisReportCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateHealthAnalysisReportCommandHandler(
        IUnitOfWork context,
        IApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _context = context;
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(CreateHealthAnalysisReportCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.PatientRepository.GetByIdAsync(request.Patientid);
        if (patient == null) return OperationResult.NotSuccessful("Patient not found");

        var physician = await ResolvePhysicianAsync(request.PhysicianId, cancellationToken);
        if (physician == null) return OperationResult.NotSuccessful("Physician not found");
        await EnsureTreatingRelationshipAsync(physician.Id, patient.Id, cancellationToken);

        var healthReport = new HealthAnalysisReport
        {
            Created = DateTime.UtcNow,
            CreatedBy = _currentUser.Email
        };

        var comment = new Comment
        {
            Created = DateTime.UtcNow,
            CreatedBy = _currentUser.Email,
            Description = request.Description?.Trim(),
            Title = request.Title?.Trim(),
            Physician = physician
        };
        healthReport.AddComment(comment);
        _context.HealthAnalysisReportRepository.Add(healthReport);
        patient.AddHealthAnalysisReport(healthReport);
        patient.LastModified = DateTime.UtcNow;
        patient.LastModifiedBy = _currentUser.Email;
        _context.PatientRepository.Update(patient);

        var commitResult = await _context.SaveChangesAsync(cancellationToken);
        return commitResult.WasSuccesful
            ? OperationResult.Successful(healthReport.Id)
            : OperationResult.NotSuccessful("Unable to save health report");
    }

    private async Task<Physician?> ResolvePhysicianAsync(int requestedPhysicianId, CancellationToken cancellationToken)
    {
        if (string.Equals(_currentUser.Role, "Physician", StringComparison.OrdinalIgnoreCase))
        {
            return await _db.Physicians.FirstOrDefaultAsync(x =>
                (_currentUser.UserId != Guid.Empty && x.IdentityUserId == _currentUser.UserId)
                || x.Email == _currentUser.Email, cancellationToken);
        }

        if (!string.Equals(_currentUser.Role, "Administrator", StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenAccessException();

        return await _db.Physicians.FirstOrDefaultAsync(x => x.Id == requestedPhysicianId, cancellationToken);
    }

    private async Task EnsureTreatingRelationshipAsync(int physicianId, int patientId, CancellationToken cancellationToken)
    {
        if (string.Equals(_currentUser.Role, "Administrator", StringComparison.OrdinalIgnoreCase))
            return;

        var hasRelationship = await _db.Appointments.AnyAsync(x =>
            x.PhysicianId == physicianId && x.PatientId == patientId, cancellationToken);
        if (!hasRelationship)
            throw new ForbiddenAccessException();
    }
}
