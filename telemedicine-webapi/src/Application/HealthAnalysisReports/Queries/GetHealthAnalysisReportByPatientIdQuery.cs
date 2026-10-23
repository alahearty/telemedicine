using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.HealthAnalysisReports.Queries;

public record GetHealthAnalysisReportByPatientIdQuery(int patientId) : IRequest<BaseResponse>;

public class GetHealthAnalysisReportByPatientIdQueryHandler : IRequestHandler<GetHealthAnalysisReportByPatientIdQuery, BaseResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetHealthAnalysisReportByPatientIdQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(GetHealthAnalysisReportByPatientIdQuery request, CancellationToken cancellationToken)
    {
        var isAdministrator = string.Equals(_currentUser.Role, "Administrator", StringComparison.OrdinalIgnoreCase);
        if (!isAdministrator)
        {
            if (!string.Equals(_currentUser.Role, "Physician", StringComparison.OrdinalIgnoreCase))
                throw new ForbiddenAccessException();

            var physician = await _db.Physicians.FirstOrDefaultAsync(x =>
                (_currentUser.UserId != Guid.Empty && x.IdentityUserId == _currentUser.UserId)
                || x.Email == _currentUser.Email, cancellationToken);
            if (physician is null)
                throw new ForbiddenAccessException();

            var treatsPatient = await _db.Appointments.AnyAsync(x =>
                x.PatientId == request.patientId && x.PhysicianId == physician.Id, cancellationToken);
            if (!treatsPatient)
                throw new ForbiddenAccessException();
        }

        var reports = await _db.Set<HealthAnalysisReport>()
            .AsNoTracking()
            .Include(x => x.Patient)
            .Include(x => x.Comments)
                .ThenInclude(x => x.Physician)
            .Where(x => x.Patient != null && x.Patient.Id == request.patientId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        var dtos = reports.Select(report => new HealthAnalysisReportDto
        {
            Id = report.Id,
            When = report.Created,
            PatientId = report.Patient?.Id ?? request.patientId,
            Comments = (report.Comments ?? Array.Empty<Comment>())
                .Where(comment => comment != null)
                .Select(comment => new CommentDto
                {
                    Id = comment.Id,
                    Title = comment.Title,
                    Description = comment.Description,
                    PhysicianId = comment.Physician?.Id ?? 0,
                    PhysicianName = comment.Physician != null
                        ? $"{comment.Physician.FirstName} {comment.Physician.LastName}".Trim()
                        : null,
                    Created = comment.Created
                })
                .ToList()
        }).ToList();

        return OperationResult.Successful(dtos);
    }
}
