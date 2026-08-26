using System.ComponentModel.DataAnnotations;

namespace TaskFlow.DTOs;

public class PartialUpdateTaskItemDto
{
    // Id is not included because it comes from the URL route (PUT /api/TaskItem/{id}), not from the request body
    [MaxLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    public string? Title { get; set; } = string.Empty; // nullable = optional for partial update
    public bool? IsCompleted { get; set; } // nullable = optional for partial update
}