using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TelemedicineServices.Queries;
public record GetTelemedicineServicesQuery:IRequest<BaseResponse>;

public class GetTelemedicineServicesQueryHandler : IRequestHandler<GetTelemedicineServicesQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetTelemedicineServicesQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BaseResponse> Handle(GetTelemedicineServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _context.TelemedicineServiceRepository.GetAllAsync();
        var response = new List<TeleMedicineServiceDto>();
        if(services != null) response = _mapper.Map<List<TeleMedicineServiceDto>>(services);

        return OperationResult.Successful(response);
    }
}