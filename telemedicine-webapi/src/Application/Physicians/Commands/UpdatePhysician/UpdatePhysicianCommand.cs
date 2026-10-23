using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Physicians.Commands.UpdatePhysician;

public record UpdatePhysicianCommand : IRequest<BaseResponse>
{
    public int Id { get; init; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? MedicalSpecialization { get; set; }
    public string? Qualifications { get; set; }
    public string? Bio { get; set; }
    public Domain.Enums.Gender? Gender { get; set; }
    public DateTime? Birth { get; set; }
    public byte[]? Avatar { get; set; }
}

public class UpdatePhysicianCommandHandler : IRequestHandler<UpdatePhysicianCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;

    public UpdatePhysicianCommandHandler(IUnitOfWork context, IIdentityService identityService, ICurrentUserService currentUser)
    {
        _context = context;
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(UpdatePhysicianCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PhysicianRepository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new NotFoundException("Physician", request.Id);

        var isOwner = (entity.IdentityUserId.HasValue && entity.IdentityUserId == _currentUser.UserId)
            || string.Equals(entity.Email, _currentUser.Email, StringComparison.OrdinalIgnoreCase);
        if (!isOwner)
            throw new ForbiddenAccessException();

        if (request.FirstName != null) entity.FirstName = request.FirstName.Trim();
        if (request.LastName != null) entity.LastName = request.LastName.Trim();
        if (request.Email != null && !string.Equals(request.Email, entity.Email, StringComparison.OrdinalIgnoreCase))
            await UpdateEmail(request.Email.Trim().ToLowerInvariant(), entity);
        if (request.Phone != null) entity.Phone = request.Phone.Trim();
        if (request.Address != null) entity.Address = request.Address.Trim();
        if (request.MedicalSpecialization != null) entity.MedicalSpecialization = request.MedicalSpecialization.Trim();
        if (request.Qualifications != null) entity.Qualifications = request.Qualifications.Trim();
        if (request.Bio != null) entity.Bio = request.Bio.Trim();
        if (request.Gender.HasValue) entity.Gender = request.Gender.Value;
        if (request.Birth.HasValue)
        {
            if (request.Birth.Value == default || request.Birth.Value.Date > DateTime.UtcNow.Date)
                return OperationResult.NotSuccessful("A valid birth date is required");
            entity.Birth = request.Birth.Value.Date;
        }
        if (request.Avatar != null) entity.Avatar = request.Avatar;
        entity.LastModified = DateTime.UtcNow;
        entity.LastModifiedBy = _currentUser.Email;

        _context.PhysicianRepository.Update(entity);

        var commitResult = await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful ? OperationResult.Successful() : OperationResult.NotSuccessful("");
    }

    private async Task UpdateEmail(string newEmail, Physician patient)
    {
        var result = await _identityService.UpdateEmailAsync(patient.Email!, newEmail);
        if (result.WasSuccesful) patient.Email = newEmail;
    }
}
