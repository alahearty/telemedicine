using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Physicians.Queries.GetPhysicians;
public record GetPhysicianQuery(int id):IRequest<BaseResponse>;

public class GetPhysicianQueryHandler : IRequestHandler<GetPhysicianQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public GetPhysicianQueryHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(GetPhysicianQuery request, CancellationToken cancellationToken)
    {
        var physician = await _context.PhysicianRepository.GetByIdAsync(request.id);
        if (physician == null) return OperationResult.NotSuccessful("Physician not found");

        var response = new PhysicianDto
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
        };

        return OperationResult.Successful(response);
    }
}
