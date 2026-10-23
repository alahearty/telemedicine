namespace telemedicine_webapi.Application.Common.Models.Authentication;

/// <summary>
/// Account registration plus the minimum profile data needed for a linked
/// Patient or Physician record. Profile fields are validated per account type.
/// </summary>
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public DateTime? Birth { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public string? License { get; set; }
    public string? MedicalSpecialization { get; set; }
    public string? Qualifications { get; set; }
    public string? Bio { get; set; }

    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}
