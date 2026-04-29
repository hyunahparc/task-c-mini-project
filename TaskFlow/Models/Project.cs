using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Models
{
	public class Project
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string? Description { get; set; }
		public DateTime CreationDate { get; set; } = DateTime.UtcNow;
		public int UserId {  get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; }

		public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
	}
}
