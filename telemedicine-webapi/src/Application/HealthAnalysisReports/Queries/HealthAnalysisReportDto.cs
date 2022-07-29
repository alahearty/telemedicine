using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.HealthAnalysisReports.Queries;
public class HealthAnalysisReportDto : IMapFrom<HealthAnalysisReport>
{
    public DateTime When { get; set; }
    public int PatientId { get; set; }
    public List<CommentDto>? Comments { get; set; }
}
