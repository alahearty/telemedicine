using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Physicians.Queries.GetPhysicians;
public class PhysicianDto:IMapFrom<Physician>
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? License { get; set; }
    public string? MedicalSpecialization { get; set; }
    public byte[]? Avatar { get; set; }
}
