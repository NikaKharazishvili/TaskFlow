using Microsoft.EntityFrameworkCore;
using TaskFlow.Common;
using TaskFlow.Data;
using TaskFlow.DTOs;
using TaskFlow.Models;

namespace TaskFlow.Services;

public class TaskItemService : ITaskItemService
{
    readonly ApplicationDbContext context;
    readonly ILogger<TaskItemService> logger;

    public TaskItemService(ApplicationDbContext context, ILogger<TaskItemService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    // Returns a paginated list of the user's tasks — every query filters by userId so users only ever see their own data
    public async Task<PagedResponse<TaskItem>> GetAllAsync(string userId, int page, int pageSize)
    {
        logger.LogInformation($"Fetching tasks - user: {userId}, page: {page}, page size: {pageSize}");

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 5;

        var query = context.TaskItems.AsNoTracking().Where(t => t.UserId == userId);
        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResponse<TaskItem>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        logger.LogInformation($"Successfully fetched {items.Count} tasks for user: {userId}");
        return result;
    }

    // Returns a single task, only if it belongs to the requesting user
    public async Task<TaskItem?> GetByIdAsync(int id, string userId)
    {
        logger.LogInformation($"Fetching task with id: {id} for user: {userId}");

        var existing = await context.TaskItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (existing == null) logger.LogWarning($"Task with id: {id} not found for user: {userId}");
        return existing;
    }

    // Creates and saves a new task
    public async Task<TaskItem> CreateAsync(TaskItem taskItem)
    {
        logger.LogInformation($"Creating new task with title: {taskItem.Title} for user: {taskItem.UserId}");

        await context.TaskItems.AddAsync(taskItem);
        await context.SaveChangesAsync();
        logger.LogInformation($"Task created successfully. Id: {taskItem.Id}");
        return taskItem;
    }

    // Fully replaces a task's fields, only if it belongs to the requesting user
    public async Task<TaskItem?> UpdateAsync(int id, string userId, UpdateTaskItemDto dto)
    {
        logger.LogInformation($"Updating task with id: {id} for user: {userId}");

        var existing = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (existing == null)
        {
            logger.LogWarning($"Task with id: {id} not found for update, user: {userId}");
            return null;
        }

        existing.Title = dto.Title;
        existing.IsCompleted = dto.IsCompleted;
        await context.SaveChangesAsync();
        logger.LogInformation($"Task with id: {id} updated successfully");
        return existing;
    }

    // Updates only the fields provided (nullable), only if the task belongs to the requesting user
    public async Task<TaskItem?> PartialUpdateAsync(int id, string userId, PartialUpdateTaskItemDto dto)
    {
        logger.LogInformation($"Partially updating task with id: {id} for user: {userId}");

        var existing = await context.TaskItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (existing == null)
        {
            logger.LogWarning($"Task with id: {id} not found for partial update, user: {userId}");
            return null;
        }

        if (!string.IsNullOrWhiteSpace(dto.Title)) existing.Title = dto.Title;
        if (dto.IsCompleted.HasValue) existing.IsCompleted = dto.IsCompleted.Value;
        await context.SaveChangesAsync();
        logger.LogInformation($"Task with id: {id} partially updated successfully");
        return existing;
    }

    // Deletes a task, only if it belongs to the requesting user
    public async Task<TaskItem?> DeleteAsync(int id, string userId)
    {
        logger.LogInformation($"Deleting task id: {id} for user: {userId}");

        var existing = await context.TaskItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (existing == null)
        {
            logger.LogWarning($"Task with id: {id} not found for delete, user: {userId}");
            return null;
        }

        context.TaskItems.Remove(existing);
        await context.SaveChangesAsync();
        logger.LogInformation($"Task with id: {id} deleted successfully");
        return existing;
    }
}