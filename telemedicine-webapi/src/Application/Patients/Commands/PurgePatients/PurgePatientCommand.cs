using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Security;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Patients.Commands.UpdatePatients;

// [Authorize(Roles = "Administrator")]
// [Authorize(Policy = "CanPurge")]
public record PurgePatientCommand : IRequest<BaseResponse>;

public class PurgePatientsCommandHandler : IRequestHandler<PurgePatientCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public PurgePatientsCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(PurgePatientCommand request, CancellationToken cancellationToken)
    {
        var patients=await _context.PatientRepository.GetAllAsync();
        _context.PatientRepository.DeleteMany(patients);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to purge patients");
    }
}
