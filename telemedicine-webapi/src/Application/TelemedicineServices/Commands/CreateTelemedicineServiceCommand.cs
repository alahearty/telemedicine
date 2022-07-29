using System.ComponentModel.DataAnnotations;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.TelemedicineServices.Commands;
public class CreateTelemedicineServiceCommand : IRequest<BaseResponse>
{
    [Required]
    public virtual string? ServiceName { get; set; }
    [Required]
    public virtual decimal Cost { get; set; }
    [Required]
    public virtual string? Description { get; set; }
}

public class CreateTelemedicineServiceHandler : IRequestHandler<CreateTelemedicineServiceCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public CreateTelemedicineServiceHandler(IUnitOfWork context)
    {
        _context = context;
    }
    public async Task<BaseResponse> Handle(CreateTelemedicineServiceCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.TelemedicineServiceRepository.Exists(t => t.ServiceName == request.ServiceName);
        if (exists) return OperationResult.NotSuccessful($"{request.ServiceName} already exist");

        var telmedService = new TelemedicineService
        {
            ServiceName = request.ServiceName,
            Cost = request.Cost,
            Description = request.Description,
            Created = DateTime.UtcNow,
            CreatedBy = null,
        };

        _context.TelemedicineServiceRepository.Add(telmedService);
        var commitResult = await _context.SaveChangesAsync();

        return commitResult.WasSuccesful ? OperationResult.Successful(telmedService.Id)
            : OperationResult.NotSuccessful("Unable to save");
    }
}
