namespace telemedicine_webapi.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public virtual int ListId { get; set; }

    public virtual string? Title { get; set; }

    public virtual string? Note { get; set; }

    public virtual PriorityLevel Priority { get; set; }

    public virtual DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value == true && _done == false)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public virtual TodoList List { get; set; } = null!;
}
