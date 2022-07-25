using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Hospitals.Commands.PurgeHospital;

public record PurgeHospitalsCommand : IRequest<BaseResponse>;

public class PurgeHospitalsCommandHandler : IRequestHandler<PurgeHospitalsCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public PurgeHospitalsCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(PurgeHospitalsCommand request, CancellationToken cancellationToken)
    {
        var hospitals = await _context.HospitalRepository.GetAllAsync();
        _context.HospitalRepository.DeleteMany(hospitals);

        var commitResult = await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful ? OperationResult.Successful() : OperationResult.NotSuccessful("unable to delete hospitals");
    }
}
