using System.ComponentModel.DataAnnotations;

namespace EmployeeClient.ViewModels
{
	public class UserViewModel
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Patronymic { get; set; }
		public string Username { get; set; }
		public string UserId { get; set; }
		public bool isBlocked { get; set; }
		public string Role { get; set; }
		public string CreditRating { get; set; }
	}
}
