using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Hospitals.Commands.DeleteHospital;

public record DeleteHospitalCommand(int Id) : IRequest<BaseResponse>;

public class DeleteHospitalCommandHandler : IRequestHandler<DeleteHospitalCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public DeleteHospitalCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(DeleteHospitalCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.HospitalRepository.GetById(request.Id);

        if (entity == null)return OperationResult.NotSuccessful($"Entity with id-{request.Id} not found");

        _context.HospitalRepository.Delete(entity);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);
        if(commitResult.WasSuccesful)return OperationResult.Successful();

        return OperationResult.NotSuccessful("unable to delete entity");
    }
}
