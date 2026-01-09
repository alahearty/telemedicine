using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.Hospitals.Queries.GetHospitals;

namespace telemedicine_webapi.Application.Hospitals.Queries;

public record GetHospitalByIdQuery(int Id) : IRequest<BaseResponse>;

public class GetHospitalByIdQueryHandler : IRequestHandler<GetHospitalByIdQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;

    public GetHospitalByIdQueryHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(GetHospitalByIdQuery request, CancellationToken cancellationToken)
    {
        var hospital = await _context.HospitalRepository.GetByIdAsync(request.Id);
        
        if (hospital == null)
        {
            return OperationResult.NotSuccessful($"Hospital with ID {request.Id} not found");
        }

        var hospitalDto = new HospitalDto(hospital.HospitalName!, hospital.Address!);
        return OperationResult.Successful(hospitalDto);
    }
}

