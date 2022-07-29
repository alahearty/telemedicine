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
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IIdentityService identityService, IMapper mapper)
    {
        _identityService = identityService;
        _mapper = mapper;
    }
    public async Task<BaseResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        //var users = await _identityService.GetAllUsersAsync();
        //var usersDtos = new List<UserDto>();
        //foreach (var user in users.Data)
        //{
        //    usersDtos.Add(new UserDto
        //    {
        //        AccountType = user.AccountType,
        //        Address = user.Address,
        //        Age = user.GetAge(),
        //        Avatar = user.Avatar,
        //        Birth = user.Birth,
        //        Email = user.Email,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Gender = user.,
        //        Phone = user.PhoneNumber
        //    });
        //}
        //return OperationResult.Successful(usersDtos);
        return OperationResult.Successful();
    }
}
