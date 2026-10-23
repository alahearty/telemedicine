using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Physicians.Queries.GetPhysicians;

public record GetCurrentPhysicianQuery : IRequest<BaseResponse>;

public class GetCurrentPhysicianQueryHandler : IRequestHandler<GetCurrentPhysicianQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentPhysicianQueryHandler(IUnitOfWork context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(GetCurrentPhysicianQuery request, CancellationToken cancellationToken)
    {
        var physicians = await _context.PhysicianRepository.FindAsync(p =>
            (_currentUser.UserId != Guid.Empty && p.IdentityUserId == _currentUser.UserId)
            || p.Email == _currentUser.Email);
        var physician = physicians.FirstOrDefault();
        if (physician == null)
            return OperationResult.NotSuccessful("Physician record not found.");

        var dto = new PhysicianDto
        {
            Id = physician.Id,
            FirstName = physician.FirstName,
            LastName = physician.LastName,
            Email = physician.Email,
            Gender = physician.Gender,
            Age = physician.GetAge(),
            Phone = physician.Phone,
            Address = physician.Address,
            License = physician.License,
            MedicalSpecialization = physician.MedicalSpecialization,
            Qualifications = physician.Qualifications,
            Bio = physician.Bio,
            Avatar = physician.Avatar,
        };
        return OperationResult.Successful(dto);
    }
}
