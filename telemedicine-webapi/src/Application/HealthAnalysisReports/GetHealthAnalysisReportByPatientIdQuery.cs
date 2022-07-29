using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.HealthAnalysisReports;
public record GetHealthAnalysisReportByPatientIdQuery(int patientId):IRequest<BaseResponse>;

public class GetHealthAnalysisReportByPatientIdQueryHandler : IRequestHandler<GetHealthAnalysisReportByPatientIdQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public GetHealthAnalysisReportByPatientIdQueryHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(GetHealthAnalysisReportByPatientIdQuery request, CancellationToken cancellationToken)
    {
        var patientHealthReports = await _context.HealthAnalysisReportRepository.FindAsync(h => h.Patient!.Id == request.patientId);
        var healthReportDtos = new List<HealthAnalysisReportDto>();

        foreach (var healthReport in patientHealthReports)
        {
            healthReportDtos.Add(new HealthAnalysisReportDto
            {
                PatientId = healthReport.Patient!.Id,
                Comments = healthReport.Comments.Select(c => new CommentDto
                {
                    Description = c.Description,
                    PhysicianId = c.Physician!.Id,
                    Title = c.Title
                }).ToList()
            });
        }

        return OperationResult.Successful(healthReportDtos);
    }
}
