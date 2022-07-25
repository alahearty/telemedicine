using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;
using System.ComponentModel.DataAnnotations;
using telemedicine_webapi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Application.Physicians.Commands.CreatePhysician;

public record CreatePhysicianCommand : IRequest<BaseResponse>
{
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public Gender Gender { get; set; }
    [Column(TypeName = "Date")]
    public DateTime Birth { get; set; }
    [Required]
    public string? Phone { get; set; }
    public string? Address { get; set; }
    [Required]
    public virtual string? License { get; set; }
    [Required]
    public virtual string? MedicalSpecialization { get; set; }
}

public class CreatePhysicianCommandHandler : IRequestHandler<CreatePhysicianCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public CreatePhysicianCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(CreatePhysicianCommand request, CancellationToken cancellationToken)
    {
        var entity = new Physician()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            Email = request.Email,
            Phone = request.Phone,
            Birth = request.Birth,
            Address = request.Address,
            License = request.License,
            MedicalSpecialization = request.MedicalSpecialization
        };

        _context.PhysicianRepository.Add(entity);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful(entity.Id):OperationResult.NotSuccessful("Unable to create physician");
    }
}
