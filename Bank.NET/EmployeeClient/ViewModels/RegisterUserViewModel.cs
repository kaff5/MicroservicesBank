using System.ComponentModel.DataAnnotations;

namespace EmployeeClient.ViewModels
{
	public class RegisterUserViewModel
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Surname { get; set; }
		[Required]
		public string Patronymic { get; set; }
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
