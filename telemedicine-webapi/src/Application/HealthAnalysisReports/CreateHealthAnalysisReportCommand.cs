using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.HealthAnalysisReports;
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

    public CreateHealthAnalysisReportCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(CreateHealthAnalysisReportCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.PatientRepository.GetByIdAsync(request.Patientid);
        if (patient == null) return OperationResult.NotSuccessful("Patient not found");

        var healthReport = new HealthAnalysisReport();

        var physician = await _context.PhysicianRepository.GetByIdAsync(request.PhysicianId);
        if (physician == null) return OperationResult.NotSuccessful("Physician not found");

        var comment = new Comment
        {
            Created = DateTime.UtcNow,
            CreatedBy = null,
            Description = request.Description,
            Title = request.Title,
            Physician = physician
        };
        healthReport.AddComment(comment);
        _context.HealthAnalysisReportRepository.Add(healthReport);
        patient.AddHealthAnalysisReport(healthReport);
        patient.LastModified = DateTime.UtcNow;
        patient.LastModifiedBy = null;
        _context.PatientRepository.Update(patient);

        var commitResult = await _context.SaveChangesAsync();
        return commitResult.WasSuccesful ? OperationResult.Successful(healthReport.Id)
            : OperationResult.NotSuccessful("Unable to save health report");
    }
}
