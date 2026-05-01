using TaskFlow.Models;

namespace TaskFlow.Dto
{
    public class ProjectTaskDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public ProjectTaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
