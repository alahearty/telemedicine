namespace telemedicine_webapi.Domain.Entities;

public class HealthAnalysisReport : BaseAuditableEntity
{
    public virtual void AddComment(Comment comment)
    {
        comment.HealthAnalysisReport = this;
        Comments.Add(comment);
    }

    public virtual void RemoveComment(Comment comment)
    {
        Comments.Remove(comment);
    }

    public virtual double? Temp { get; set; }
    public virtual int? HeartRate { get; set; }
    public virtual string? Pressure { get; set; }
    public virtual DateTime When => DateTime.UtcNow;
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
