using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.HealthAnalysisReports;
public class CommentDto:IMapFrom<Comment>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PhysicianId { get; set; }
}
