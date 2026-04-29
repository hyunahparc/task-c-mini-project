using Microsoft.EntityFrameworkCore;
using TaskFlow.Models;

namespace TaskFlow.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}