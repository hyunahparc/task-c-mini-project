using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Models
{
	public enum ProjectTaskStatus
	{
		Todo,
		InProgress,
		Done
	}

	public class ProjectTask
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public ProjectTaskStatus Status { get; set; }
		public int ProjectId { get; set; }
		[ForeignKey("ProjectId")]
		public Project Project { get; set; }
		public DateTime? DueDate { get; set; }

		public List<Comment> Comments { get; set; } = new List<Comment>();
	}
}
