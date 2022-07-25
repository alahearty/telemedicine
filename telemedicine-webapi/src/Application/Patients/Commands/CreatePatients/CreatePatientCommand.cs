using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Patients.Commands.CreatePatients;

public record CreatePatientCommand : IRequest<BaseResponse>
{
    [Required]
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
}

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public CreatePatientCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var entity = new Patient
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address,
            Birth = request.Birth,
            Phone = request.Phone,
            Email = request.Email,
            Gender = request.Gender
        };

        _context.PatientRepository.Add(entity);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful(entity.Id):OperationResult.NotSuccessful("Unable to save Todo list");
    }
}
