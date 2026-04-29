using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Models
{
	public enum UserRoles
	{
		User,
		Admin
	}

	public class User
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string PasswordHash { get; set; }
		public UserRoles Role { get; set; } = UserRoles.User;

		public List<Project> Projects { get; set; } = new List<Project>();
	}
}
