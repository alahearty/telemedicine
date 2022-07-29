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

    public virtual DateTime When => DateTime.UtcNow;
    public virtual Patient? Patient { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
