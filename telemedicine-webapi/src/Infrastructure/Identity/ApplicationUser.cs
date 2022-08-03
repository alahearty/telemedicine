using Microsoft.AspNetCore.Identity;

namespace telemedicine_webapi.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public virtual string? Role { get; set; }
}
