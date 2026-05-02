using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Data;
using TaskFlow.Dto;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    [ApiController]
    [Authorize]
    [EnableCors("AllowFrontend")]
    [Route("api/tasks")]
    public class ProjectTaskController : ControllerBase
    {
        private readonly ApiContext _context;

        public ProjectTaskController(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all tasks for the authenticated task owner and admin.
        /// </summary>
        /// <returns>List of task DTOs</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProjectTaskDto>>> GetAllTasks()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            List<ProjectTask> tasks = await _context.ProjectTasks
                .Where(t => t.Project.UserId == userId || isAdmin)
                .ToListAsync();

            List<ProjectTaskDto> projectTaskDto = tasks.Select(t => new ProjectTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status
            }).ToList();

            return Ok(projectTaskDto);
        }

        /// <summary>
        /// Retrieves a task by its ID for the authenticated task owner and admin.
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Task details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskDto>> GetTaskById(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            ProjectTask? projectTask = await _context.ProjectTasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);

            if (projectTask == null)
            {
                return NotFound("Task not found.");
            }

            if (!isAdmin && projectTask.Project.UserId != userId)
            {
                return Forbid();
            }

            ProjectTaskDto projectTaskDto = new ProjectTaskDto
            {
                Id = projectTask.Id,
                Title = projectTask.Title,
                Status = projectTask.Status
            };

            return Ok(projectTaskDto);
        }

        /// <summary>
        /// Creates a new task under a project.
        /// Only the project owner or admin can create tasks.
        /// </summary>
        /// <param name="projectTaskDto">Task data</param>
        /// <returns>Created task</returns>
        [HttpPost]
        public async Task<ActionResult<ProjectTaskDto>> CreateTask([FromBody] ProjectTaskDto projectTaskDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectTaskDto.ProjectId);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            if (!isAdmin && project.Id != userId)
            {
                return Forbid();
            }

            ProjectTask projectTask = new ProjectTask
            {
                Title = projectTaskDto.Title,
                Status = projectTaskDto.Status,
                DueDate = projectTaskDto.DueDate,
                ProjectId = projectTaskDto.ProjectId,
            };
            
            _context.ProjectTasks.Add(projectTask);
            await _context.SaveChangesAsync();

            var result = new ProjectTaskDto
            {
                Id = projectTask.Id,
                Title = projectTaskDto.Title,
                Status = projectTask.Status,
                DueDate = projectTask.DueDate,
                ProjectId = projectTask.ProjectId
            };

            return CreatedAtAction(nameof(GetTaskById), new { id = projectTask.Id }, result);
        }

        /// <summary>
        /// Updates an existing task.
        /// Only the project/task owner or admin can update it.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="projectTaskDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProjectTask(int id, [FromBody] ProjectTaskDto projectTaskDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectTask = await _context.ProjectTasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);

            if (projectTask == null)
            {
                return NotFound("Task not found.");
            }

            if (!isAdmin && projectTask.Project.UserId != userId)
            {
                return Forbid();
            }

            projectTask.Title = projectTaskDto.Title;
            projectTask.Status = projectTaskDto.Status;
            projectTask.DueDate = projectTaskDto.DueDate;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a task.
        /// Only the project/task owner or admin can delete it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProjectTask(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            ProjectTask? projectTask = await _context.ProjectTasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);

            if (projectTask == null)
            {
                return NotFound("Task not found.");
            }

            if (!isAdmin && projectTask.Project.UserId != userId)
            {
                return Forbid();
            }

            _context.ProjectTasks.Remove(projectTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}