using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Physicians.Queries.GetPhysicians;
public record GetPhysiciansQuery():IRequest<BaseResponse>;

public class GetPhysiciansQueryHandler : IRequestHandler<GetPhysiciansQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public GetPhysiciansQueryHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(GetPhysiciansQuery request, CancellationToken cancellationToken)
    {
        var physicians = await _context.PhysicianRepository.GetAllAsync();
        var response = new List<PhysicianDto>();

        if(physicians != null)
        {
            foreach (var physician in physicians)
            {
                response.Add(new PhysicianDto
                {
                    Address = physician.Address,
                    Age = physician.GetAge(),
                    Avatar = physician.Avatar,
                    Email = physician.Email,
                    FirstName = physician.FirstName,
                    Gender = physician.Gender,
                    Id = physician.Id,
                    LastName = physician.LastName,
                    License = physician.License,
                    MedicalSpecialization = physician.MedicalSpecialization,
                    Phone = physician.Phone
                });
            }
        }
        return OperationResult.Successful(response);
    }
}
