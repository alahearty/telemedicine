﻿namespace telemedicine_webapi.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity<int>
{
    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}
