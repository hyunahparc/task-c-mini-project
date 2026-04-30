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
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly ApiContext _context;

        public ProjectController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectDto>>> GetAllProjects()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            List<Project> projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync();

            List<ProjectDto> projectDto = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            }).ToList();

            return Ok(projectDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            
            if(project == null)
            {
                return NotFound("Project not found.");
            }

            ProjectDto projectDto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };

            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] ProjectDto projectDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Project project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                UserId = userId,
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var result = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };

            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProject(int id, [FromBody] ProjectDto projectDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            project.Name = projectDto.Name;
            project.Description = projectDto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}