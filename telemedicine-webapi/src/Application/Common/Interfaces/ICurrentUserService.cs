namespace telemedicine_webapi.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
