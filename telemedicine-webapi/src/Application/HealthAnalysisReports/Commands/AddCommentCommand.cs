using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.HealthAnalysisReports.Commands;

public class AddCommentCommand : IRequest<BaseResponse>
{
    public int HealthReportId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PhysicianId { get; set; }
}

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, BaseResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddCommentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var healthReport = await _db.Set<HealthAnalysisReport>()
            .Include(x => x.Patient)
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == request.HealthReportId, cancellationToken);
        if (healthReport == null) return OperationResult.NotSuccessful("Health report not found");

        var physician = await ResolvePhysicianAsync(request.PhysicianId, cancellationToken);
        if (physician == null) return OperationResult.NotSuccessful("Physician not found");

        if (healthReport.Patient != null)
            await EnsureTreatingRelationshipAsync(physician.Id, healthReport.Patient.Id, cancellationToken);

        var comment = new Comment
        {
            Created = DateTime.UtcNow,
            CreatedBy = _currentUser.Email,
            Description = request.Description?.Trim(),
            Title = request.Title?.Trim(),
            Physician = physician
        };
        healthReport.AddComment(comment);
        healthReport.LastModified = DateTime.UtcNow;
        healthReport.LastModifiedBy = _currentUser.Email;

        var commitResult = await _db.SaveChangesAsync(cancellationToken);
        return commitResult > 0
            ? OperationResult.Successful(healthReport.Id)
            : OperationResult.NotSuccessful("Unable to update health report");
    }

    private async Task<Physician?> ResolvePhysicianAsync(int requestedPhysicianId, CancellationToken cancellationToken)
    {
        if (string.Equals(_currentUser.Role, "Physician", StringComparison.OrdinalIgnoreCase))
        {
            var physician = await _db.Physicians.FirstOrDefaultAsync(x =>
                (_currentUser.UserId != Guid.Empty && x.IdentityUserId == _currentUser.UserId)
                || x.Email == _currentUser.Email, cancellationToken);
            if (physician == null) throw new ForbiddenAccessException();
            return physician;
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
