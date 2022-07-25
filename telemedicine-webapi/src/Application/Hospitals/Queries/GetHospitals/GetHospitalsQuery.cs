using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Hospitals.Queries.GetHospitals;

public record GetHospitalsQuery():IRequest<BaseResponse>;

public class GetHospitalsQueryHandler : IRequestHandler<GetHospitalsQuery, BaseResponse>
{
    private readonly IUnitOfWork _unitOfwork;

    public GetHospitalsQueryHandler(IUnitOfWork unitOfwork)
    {
        _unitOfwork = unitOfwork;
    }
    public async Task<BaseResponse> Handle(GetHospitalsQuery request, CancellationToken cancellationToken)
    {
        var hospitals = await _unitOfwork.HospitalRepository.GetAllAsync();
        var hospitalDtos = new List<HospitalDto>();

        if(hospitals != null)
        {
            foreach (var hospital in hospitals)
            {
                hospitalDtos.Add(new HospitalDto(hospital.HospitalName!, hospital.Address!));
            }
        }
        return OperationResult.Successful(hospitalDtos);
    }
}
