using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Mappers;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Controllers;

[ApiController, Route("api/[controller]"), Authorize] // Authorize here means every action below requires a valid JWT
public class TaskItemController : ControllerBase
{
    readonly ITaskItemService taskItemService;
    string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!; // Pulls the logged-in user's Id out of the JWT claims (set by TokenService when the token was created)

    public TaskItemController(ITaskItemService taskItemService) => this.taskItemService = taskItemService;

    [HttpGet] public async Task<IActionResult> GetAll(int page = 1, int pageSize = 5) => Ok((await taskItemService.GetAllAsync(CurrentUserId, page, pageSize)).ToPagedDto());

    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id) => await taskItemService.GetByIdAsync(id, CurrentUserId) is TaskItem taskItem ? Ok(taskItem.ToTaskItemDto()) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskItemDto dto)
    {
        var taskItem = dto.ToTaskItem(CurrentUserId);
        var newTaskItem = await taskItemService.CreateAsync(taskItem);
        return CreatedAtAction(nameof(GetById), new { id = newTaskItem.Id }, newTaskItem.ToTaskItemDto());
    }

    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, UpdateTaskItemDto dto) => await taskItemService.UpdateAsync(id, CurrentUserId, dto) is TaskItem taskItem ? Ok(taskItem.ToTaskItemDto()) : NotFound();

    [HttpPatch("{id:int}")] public async Task<IActionResult> PartialUpdate(int id, PartialUpdateTaskItemDto dto) => await taskItemService.PartialUpdateAsync(id, CurrentUserId, dto) is TaskItem taskItem ? Ok(taskItem.ToTaskItemDto()) : NotFound();

    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) => await taskItemService.DeleteAsync(id, CurrentUserId) != null ? NoContent() : NotFound();
}