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

        /// <summary>
        /// Retrieves all projects for the authenticated project owner and admin.
        /// </summary>
        /// <returns>List of project DTOs</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProjectDto>>> GetAllProjects()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            List<Project> projects = await _context.Projects
                .Where(p => p.UserId == userId || isAdmin)
                .ToListAsync();

            List<ProjectDto> projectDto = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            }).ToList();

            return Ok(projectDto);
        }

        /// <summary>
        /// Retrieves a project by its ID for the authenticated project owner and admin.
        /// </summary>
        /// <param name="id">Projet ID</param>
        /// <returns>Project details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && (p.UserId == userId || isAdmin));
            
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

        /// <summary>
        /// Creates a new project for the authenticated user and admin.
        /// </summary>
        /// <param name="projectDto">Project data</param>
        /// <returns>Created project</returns>
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

        /// <summary>
        /// Updates an existing project for the authenticated project owner and admin.
        /// </summary>
        /// <param name="id">Projet Id</param>
        /// <param name="projectDto">Update project data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProject(int id, [FromBody] ProjectDto projectDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && (p.UserId == userId || isAdmin));

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            project.Name = projectDto.Name;
            project.Description = projectDto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a project for the authenticated project owner and admin.
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isAdmin = User.IsInRole("Admin");

            Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && (p.UserId == userId || isAdmin));

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