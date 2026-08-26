using System.ComponentModel.DataAnnotations;

namespace TaskFlow.DTOs;

public class CreateTaskItemDto
{
    // Id is not included because the database generates it automatically when creating a new task
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}