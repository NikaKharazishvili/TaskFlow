using System.ComponentModel.DataAnnotations;

namespace TaskFlow.DTOs;

public class UpdateTaskItemDto
{
    // Id is not included because it comes from the URL route (PUT /api/TaskItem/{id}), not from the request body
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}