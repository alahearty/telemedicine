using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Admin.Queries.GetUsers;

public class UserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public Gender Gender { get; set; }
    public DateTime Birth { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public byte[]? Avatar { get; set; }
    public int? Age { get; set; }
    public AccountType AccountType { get; set; }
}
