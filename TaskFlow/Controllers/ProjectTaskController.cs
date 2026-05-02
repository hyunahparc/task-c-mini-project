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

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskDto>> GetTaskById(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            ProjectTask? projectTask = await _context.ProjectTasks.FirstOrDefaultAsync(t => t.Id == id && (t.Project.UserId == userId || isAdmin));

            if (projectTask == null)
            {
                return NotFound("Task not found.");
            }

            ProjectTaskDto projectTaskDto = new ProjectTaskDto
            {
                Id = projectTask.Id,
                Title = projectTask.Title,
                Status = projectTask.Status
            };

            return Ok(projectTaskDto);
        }

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
    }
}