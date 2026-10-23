using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Patients.Commands.UpdatePatients;

public record UpdatePatientCommand : IRequest<BaseResponse>
{
    public int Id { get; init; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Domain.Enums.Gender? Gender { get; set; }
    public DateTime? Birth { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public byte[]? Avatar { get; set; }
}

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;

    public UpdatePatientCommandHandler(IUnitOfWork context, IIdentityService identityService, ICurrentUserService currentUser)
    {
        _context = context;
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PatientRepository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new NotFoundException("Patient", request.Id);

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
        if (request.Gender.HasValue) entity.Gender = request.Gender.Value;
        if (request.Birth.HasValue)
        {
            if (request.Birth.Value == default || request.Birth.Value.Date > DateTime.UtcNow.Date)
                return OperationResult.NotSuccessful("A valid birth date is required");
            entity.Birth = request.Birth.Value.Date;
        }
        if (request.EmergencyContactName != null) entity.EmergencyContactName = request.EmergencyContactName.Trim();
        if (request.EmergencyContactPhone != null) entity.EmergencyContactPhone = request.EmergencyContactPhone.Trim();
        if (request.Avatar != null) entity.Avatar = request.Avatar;
        entity.LastModified = DateTime.UtcNow;
        entity.LastModifiedBy = _currentUser.Email;

        _context.PatientRepository.Update(entity);

        var commitResult =await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("");
    }

    private async Task UpdateEmail(string newEmail, Patient patient)
    {
        var result = await _identityService.UpdateEmailAsync(patient.Email!, newEmail);
        if (result.WasSuccesful) patient.Email = newEmail;
    }
}
