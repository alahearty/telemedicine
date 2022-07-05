using telemedicine_webapi.Application.Common.Interfaces;

namespace telemedicine_webapi.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
