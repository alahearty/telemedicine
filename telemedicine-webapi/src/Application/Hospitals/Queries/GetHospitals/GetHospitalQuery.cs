using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Hospitals.Queries.GetHospitals;

public record GetHospitalQuery(int id) : IRequest<BaseResponse>;

public class GetHospitalQueryHandler : IRequestHandler<GetHospitalQuery, BaseResponse>
{
    private readonly IUnitOfWork _unitOfwork;

    public GetHospitalQueryHandler(IUnitOfWork unitOfwork)
    {
        _unitOfwork = unitOfwork;
    }

    public async Task<BaseResponse> Handle(GetHospitalQuery request, CancellationToken cancellationToken)
    {
        var hospital = await _unitOfwork.HospitalRepository.GetByIdAsync(request.id);
        var hospitalDto = new HospitalDto(hospital?.HospitalName!, hospital?.Address!);
        return OperationResult.Successful(hospitalDto);
    }
}
