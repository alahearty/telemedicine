using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Admin.Queries.GetUsers;
public record GetUsersQuery : IRequest<BaseResponse>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BaseResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var patients = await _context.PatientRepository.GetAllAsync();
        var physicians = await _context.PhysicianRepository.GetAllAsync();
        var usersDtos = new List<UserDto>();
        foreach (var patient in patients)
        {
            usersDtos.Add(new UserDto
            {
                AccountType = patient.AccountType,
                Address = patient.Address,
                Age = patient.GetAge(),
                Avatar = patient.Avatar,
                Birth = patient.Birth,
                Email = patient.Email,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Gender = patient.Gender,
                Phone = patient.Phone
            });
        }
        foreach (var physician in physicians)
        {
            usersDtos.Add(new UserDto
            {
                AccountType = physician.AccountType,
                Address = physician.Address,
                Age = physician.GetAge(),
                Avatar = physician.Avatar,
                Birth = physician.Birth,
                Email = physician.Email,
                FirstName = physician.FirstName,
                LastName = physician.LastName,
                Gender = physician.Gender,
                Phone = physician.Phone
            });
        }
        return OperationResult.Successful(usersDtos);
    }
}
