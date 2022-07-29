using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Admin.Queries.GetTelemedicinePayment;
public record GetTelemedicinePaymentQuery : IRequest<BaseResponse>;

public class GetTelemedicinePaymentQueryHandler : IRequestHandler<GetTelemedicinePaymentQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetTelemedicinePaymentQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BaseResponse> Handle(GetTelemedicinePaymentQuery request, CancellationToken cancellationToken)
    {
        var telemedicinePayments = await _context.TelemedicinePaymentRepository.GetAllAsync();
        var telemedicinePaymentDtos = new List<TelemedicinePaymentDto>();
        foreach (var telemedicinePayment in telemedicinePayments)
        {
            var service = new TelemedicineServiceDto { Cost = telemedicinePayment.Service?.Cost, Description = telemedicinePayment.Service?.Description, ServiceName = telemedicinePayment.Service?.ServiceName };
            telemedicinePaymentDtos.Add(new TelemedicinePaymentDto
            {
                AmountPaid = telemedicinePayment.AmountPaid,
                IsPaid = telemedicinePayment.IsPaid,
                Service = service
            });
        }

        var telemedicinePaymentQueryDtos = new TelemedicinePaymentQueryDto
        {
            TelemedicinePayments = telemedicinePaymentDtos,
            Count = telemedicinePaymentDtos.Count(),
            TotalAmountPaid = telemedicinePaymentDtos.Select(x => x.AmountPaid).Sum(),
            NoOfNotPaidTransaction = telemedicinePaymentDtos.Select(x => x.IsPaid == false).Count(),
            NoOfPaidTransaction = telemedicinePaymentDtos.Select(x => x.IsPaid).Count()
        };

        return OperationResult.Successful(telemedicinePaymentQueryDtos);
    }
}
