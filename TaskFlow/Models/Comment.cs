using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Models
{
	public class Comment
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Content { get; set; }
		public int TaskId { get; set; }
		[ForeignKey("TaskId")]
		public ProjectTask Task { get; set; }
	}
}