namespace telemedicine_webapi.Application.Common.Models.Authentication;
public record RegisterRequest
(
    string Email,
    string Password,
    string AccountType
);
