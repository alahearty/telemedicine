namespace telemedicine_webapi.Application.Common.Models.Authentication;
public record RegisterRequest
(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string AccountType
);
