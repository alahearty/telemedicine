using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Patients.Commands.UpdatePatients;

public record UpdatePatientCommand : IRequest<BaseResponse>
{
    public int Id { get; init; }

    public string? Name { get; init; }
}

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public UpdatePatientCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.PatientRepository.GetById(request.Id);

        if (entity == null)return OperationResult.NotSuccessful($"Patient with Id-{request.Id} not found");

        entity.FirstName = request.Name;

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("");
    }
}
