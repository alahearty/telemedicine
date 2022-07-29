using MediatR;
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
    private readonly IUnitOfWork _context;

    public AddCommentCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }
    public async Task<BaseResponse> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var healthReport = await _context.HealthAnalysisReportRepository.GetByIdAsync(request.HealthReportId);
        if (healthReport == null) return OperationResult.NotSuccessful("Health report not found");

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
        healthReport.LastModified = DateTime.UtcNow;
        healthReport.LastModifiedBy = null;

        _context.HealthAnalysisReportRepository.Update(healthReport);

        var commitResult = await _context.SaveChangesAsync();
        return commitResult.WasSuccesful ? OperationResult.Successful(healthReport.Id)
            : OperationResult.NotSuccessful("Unable to update health report");
    }
}
