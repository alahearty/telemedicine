using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Admin.Queries.GetTelemedicineService;
public record GetTelemedicineServiceQuery : IRequest<BaseResponse>;

public class GetTelemedicineServiceQueryHandler : IRequestHandler<GetTelemedicineServiceQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetTelemedicineServiceQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BaseResponse> Handle(GetTelemedicineServiceQuery query, CancellationToken cancellationToken)
    {
        var telemedicineServices = await _context.TelemedicineServiceRepository.GetAllAsync();
        var telemedicineServiceDtos = _mapper.Map<List<TelemedicineServiceDto>>(telemedicineServices);

        return OperationResult.Successful(telemedicineServiceDtos);
    }
}
