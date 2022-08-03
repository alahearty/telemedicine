using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Admin.Queries.GetTelemedicinePayment;
public class TelemedicinePaymentQueryDto
{
    public List<TelemedicinePaymentDto>? TelemedicinePayments { get; set; }
    public decimal? TotalAmountPaid { get; set; }
    public double? Count { get; set; }
    public double? NoOfPaidTransaction { get; set; }
    public double? NoOfNotPaidTransaction { get; set; }
}

public class TelemedicineServiceDto
{
    public string? ServiceName { get; set; }
    public decimal? Cost { get; set; }
    public string? Description { get; set; }
}
public class TelemedicinePaymentDto
{
    public TelemedicineServiceDto? Service { get; set; }
    public decimal? AmountPaid { get; set; }
    public bool? IsPaid { get; set; }
}
