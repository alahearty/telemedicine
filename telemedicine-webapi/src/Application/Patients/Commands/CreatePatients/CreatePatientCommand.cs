using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using telemedicine_webapi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

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
    public AccountType AccountType { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _db;

    public CreatePatientCommandHandler(IUnitOfWork context, ICurrentUserService currentUser, IApplicationDbContext db)
    {
        _context = context;
        _currentUser = currentUser;
        _db = db;
    }

    public async Task<BaseResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == Guid.Empty || !string.Equals(request.Email?.Trim(), _currentUser.Email?.Trim(), StringComparison.OrdinalIgnoreCase))
            return OperationResult.NotSuccessful("The profile email must match the authenticated account");
        if (request.Birth == default || request.Birth.Date > DateTime.UtcNow.Date)
            return OperationResult.NotSuccessful("A valid birth date is required");
        if (await _db.Patients.AnyAsync(x => x.IdentityUserId == _currentUser.UserId || x.Email == _currentUser.Email, cancellationToken))
            return OperationResult.NotSuccessful("A patient profile already exists for this account");
        var entity = new Patient
        {
            IdentityUserId = _currentUser.UserId,
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            Address = request.Address?.Trim(),
            Birth = request.Birth.Date,
            Phone = request.Phone?.Trim(),
            Email = _currentUser.Email!.Trim().ToLowerInvariant(),
            Gender = request.Gender,
            EmergencyContactName = request.EmergencyContactName?.Trim(),
            EmergencyContactPhone = request.EmergencyContactPhone?.Trim(),
            AccountType = AccountType.PATIENT,
            CreatedBy = _currentUser.Email,
            Created = DateTime.UtcNow
        };

        _context.PatientRepository.Add(entity);

        try
        {
            var commitResult = await _context.SaveChangesAsync(cancellationToken);
            return commitResult.WasSuccesful
                ? OperationResult.Successful(entity.Id)
                : OperationResult.NotSuccessful("Unable to save patient profile");
        }
        catch (DbUpdateException)
        {
            return OperationResult.NotSuccessful("A patient profile already exists for this account");
        }
    }
}
