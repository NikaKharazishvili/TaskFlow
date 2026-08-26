namespace TaskFlow.Models;

/// <summary>Represents a single task belonging to a User.</summary>
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }

    public string UserId { get; set; } = string.Empty; // Foreign key linking this task to its owner
}