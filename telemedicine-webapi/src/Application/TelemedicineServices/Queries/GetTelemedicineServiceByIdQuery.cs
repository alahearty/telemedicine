using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TelemedicineServices.Queries;

public record GetTelemedicineServiceByIdQuery(int Id) : IRequest<BaseResponse>;

public class GetTelemedicineServiceByIdQueryHandler : IRequestHandler<GetTelemedicineServiceByIdQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetTelemedicineServiceByIdQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> Handle(GetTelemedicineServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await _context.TelemedicineServiceRepository.GetByIdAsync(request.Id);
        
        if (service == null)
        {
            return OperationResult.NotSuccessful($"Telemedicine service with ID {request.Id} not found");
        }

        var serviceDto = _mapper.Map<TeleMedicineServiceDto>(service);
        return OperationResult.Successful(serviceDto);
    }
}

