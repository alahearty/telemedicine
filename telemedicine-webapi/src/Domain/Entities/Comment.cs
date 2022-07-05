using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class Comment : BaseAuditableEntity
{
    public string? CommentText { get; set; }
    public DateTime LastModified { get; set; }
    public virtual Physician Doctor { get; set; }
}