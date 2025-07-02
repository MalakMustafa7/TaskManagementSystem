using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagement.Api.DTOs;
using TeamTaskManagement.Api.Errors;
using TeamTaskManagement.Api.Helper;
using TeamTaskManagement.Core.Entities.Identity;
using TeamTaskManagement.Core.Repositories;
using TeamTaskManagement.Core.Service;
using TeamTaskManagement.Core.Specifications;

namespace TeamTaskManagement.Api.Controllers
{ 
    public class TaskController : ApiBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITaskService _taskService;

        public TaskController(IUnitOfWork unitOfWork,IMapper mapper,ITaskService taskService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _taskService = taskService;
        }
        [Authorize(Roles = "TeamLeader,Admin")]
        [HttpPost]
        public async Task<ActionResult<TaskTemplateDTO>> CreateTask(TaskTemplateDTO model) {
           var task = _mapper.Map<TaskTemplate>(model);
          await _unitOfWork.Repository<TaskTemplate>().AddAsync(task);
            var Result = await _unitOfWork.CompeleteAsync();
            if(Result<=0) return BadRequest(new ApiResponse(400, "Failed to create the Task"));
            var dto = _mapper.Map<TaskTemplateDTO>(task);
            return Ok(dto);
        }
        [Authorize(Roles = "TeamLeader")]
        [HttpPost("assigntask")]
        public async Task<ActionResult<TaskAssignDTO>> AssignTask(TaskAssignDTO model)
        {
            if (!await IsTeamLeaderOfTeam(model.TeamId))
            {
                return Forbid();
            }
            var assignedTask =await _taskService.AssignTaskAsync(model.AppUserId,model.TeamId,model.TaskTemplateId);
            if (assignedTask == null)
                return BadRequest(new ApiResponse(400, "Task already assigned or user is not part of the team."));
            var dto = _mapper.Map<TaskAssignDTO>(assignedTask);
            return Ok(dto);

        }

        [Authorize(Roles = "TeamLeader")]
        [CachedAttribute(100)]
        [HttpGet("getteamtasks/{teamId}")]
        public async Task<ActionResult<IReadOnlyList<TeamTaskViewDTO>>> GetTeamTasks(int teamId)
        {
            var tasks = await _taskService.GetTeamTaskAsync(teamId);
            if (tasks == null) return NotFound(new ApiResponse(404,"There is No tasks with this id"));
            var dto = _mapper.Map<IReadOnlyList<TeamTaskViewDTO>>(tasks);
            return Ok(dto);
        }
        [Authorize]
        [CachedAttribute(100)]
        [HttpGet("usertasks/{userId}")]
        public async Task<ActionResult<IReadOnlyList<UserTaskViewDTO>>> GetUserTasks(string userId)
        {
            var currentuserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine(currentuserId);
            if (!string.Equals(currentuserId, userId, StringComparison.OrdinalIgnoreCase)
                             && !User.IsInRole("Admin")
                             && !User.IsInRole("TeamLeader"))
            {
                return Forbid();
            }
            var tasks = await _taskService.GetUserTasksAsync(userId);
            if (tasks == null) return NotFound(new ApiResponse(404, "No tasks Found"));
            var dto = _mapper.Map<IReadOnlyList<UserTaskViewDTO>>(tasks);
            return Ok(dto);
        }
        [Authorize(Roles = "TeamLeader,Admin")]
        [HttpPut("edittask/{id}")]
        public async Task<ActionResult<TaskTemplateDTO>> EditTask(int id,TaskTemplateDTO model)
        {
            var mappedTask = _mapper.Map<TaskTemplate>(model);
            var updatedTask = await _taskService.UpdateTaskTempAsync(id,mappedTask);
            if(updatedTask is null) return NotFound(new ApiResponse(404, "Task not found"));
            var returnedTask = _mapper.Map<TaskTemplateDTO>(updatedTask);
            return Ok(returnedTask);
        }
        [Authorize(Roles = "TeamLeader,Admin")]
        [HttpPut("setstatus")]
        public async Task<ActionResult<string>> SetTaskStatus(SetTaskStatusDTO model)
        {
            var result =await _taskService.UpdateStatusAsync(model.TaskAssignmentId, model.Status);
            if (!result) return NotFound(new ApiResponse(404, "Task not found"));
            return Ok("Task Status updated successfully");
        }
        [Authorize(Roles = "TeamLeader,Admin")]
        [HttpDelete("{taskId}")]
        public async Task<ActionResult<string>> DeleteTask(int taskId)
        {
            var result = await _taskService.DeleteTaskTemplateAsync(taskId);
            if (!result) return NotFound(new ApiResponse(404, "Task not found"));
            return Ok("Task deleted successfully");
        }

        private async Task<bool> IsTeamLeaderOfTeam(int teamId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(currentUserId) || !User.IsInRole("TeamLeader"))
                return false;
            var spec = new UserTeamSpec(teamId, currentUserId);
            var membership = await _unitOfWork.Repository<UserTeam>().GetEntityWithSpecAsync(spec);

            return membership != null;
        }



    }
}
