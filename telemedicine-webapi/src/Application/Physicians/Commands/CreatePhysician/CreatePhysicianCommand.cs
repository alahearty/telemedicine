using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;
using System.ComponentModel.DataAnnotations;
using telemedicine_webapi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
    public string? Qualifications { get; set; }
    public string? Bio { get; set; }
    public virtual AccountType AccountType { get; set; }
}

public class CreatePhysicianCommandHandler : IRequestHandler<CreatePhysicianCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _db;

    public CreatePhysicianCommandHandler(IUnitOfWork context, ICurrentUserService currentUser, IApplicationDbContext db)
    {
        _context = context;
        _currentUser = currentUser;
        _db = db;
    }

    public async Task<BaseResponse> Handle(CreatePhysicianCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == Guid.Empty || !string.Equals(request.Email?.Trim(), _currentUser.Email?.Trim(), StringComparison.OrdinalIgnoreCase))
            return OperationResult.NotSuccessful("The profile email must match the authenticated account");
        if (request.Birth == default || request.Birth.Date > DateTime.UtcNow.Date)
            return OperationResult.NotSuccessful("A valid birth date is required");
        if (string.IsNullOrWhiteSpace(request.License) || string.IsNullOrWhiteSpace(request.MedicalSpecialization))
            return OperationResult.NotSuccessful("Licence number and medical speciality are required");
        if (await _db.Physicians.AnyAsync(x => x.IdentityUserId == _currentUser.UserId || x.Email == _currentUser.Email || x.License == request.License, cancellationToken))
            return OperationResult.NotSuccessful("A physician profile or licence already exists");
        var entity = new Physician()
        {
            IdentityUserId = _currentUser.UserId,
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            Gender = request.Gender,
            Email = _currentUser.Email!.Trim().ToLowerInvariant(),
            Phone = request.Phone?.Trim(),
            Birth = request.Birth.Date,
            Address = request.Address?.Trim(),
            License = request.License?.Trim(),
            MedicalSpecialization = request.MedicalSpecialization?.Trim(),
            Qualifications = request.Qualifications?.Trim(),
            Bio = request.Bio?.Trim(),
            AccountType = AccountType.PHYSICIAN,
            Created = DateTime.UtcNow,
            CreatedBy = _currentUser.Email
        };

        _context.PhysicianRepository.Add(entity);

        try
        {
            var commitResult = await _context.SaveChangesAsync(cancellationToken);
            return commitResult.WasSuccesful
                ? OperationResult.Successful(entity.Id)
                : OperationResult.NotSuccessful("Unable to create physician");
        }
        catch (DbUpdateException)
        {
            return OperationResult.NotSuccessful("A physician profile or licence already exists");
        }
    }
}
