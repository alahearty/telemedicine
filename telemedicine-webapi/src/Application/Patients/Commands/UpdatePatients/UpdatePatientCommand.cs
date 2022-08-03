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

        if (entity == null)return OperationResult.NotSuccessful($"Patient with Id-{request.Id} not found");

        if (request.FirstName != null) entity.FirstName = request.FirstName;
        if (request.LastName != null) entity.LastName = request.LastName;
        if (request.Email != null) await UpdateEmail(request.Email, entity);
        if (request.Phone != null) entity.Phone = request.Phone;
        if (request.Avatar != null) entity.Avatar = request.Avatar;
        entity.LastModified = DateTime.UtcNow;
        entity.LastModifiedBy = entity.Email;

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
