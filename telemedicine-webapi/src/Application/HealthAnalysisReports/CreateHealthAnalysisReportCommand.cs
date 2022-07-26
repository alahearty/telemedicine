using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.HealthAnalysisReports;
public class CreateHealthAnalysisReportCommand:IRequest<BaseResponse>
{
    public virtual double Temp { get; set; }
    public virtual int HeartRate { get; set; }
    public virtual string? Pressure { get; set; }
}
