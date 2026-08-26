using TaskFlow.Common;
using TaskFlow.DTOs;
using TaskFlow.Models;

namespace TaskFlow.Services;

/// <summary>Handles CRUD operations for TaskItems, scoped to the owning user.</summary>
public interface ITaskItemService
{
    Task<PagedResponse<TaskItem>> GetAllAsync(string userId, int page, int pageSize);
    Task<TaskItem?> GetByIdAsync(int id, string userId);
    Task<TaskItem> CreateAsync(TaskItem taskItem);
    Task<TaskItem?> UpdateAsync(int id, string userId, UpdateTaskItemDto taskItem);
    Task<TaskItem?> PartialUpdateAsync(int id, string userId, PartialUpdateTaskItemDto taskItem);
    Task<TaskItem?> DeleteAsync(int id, string userId);
}