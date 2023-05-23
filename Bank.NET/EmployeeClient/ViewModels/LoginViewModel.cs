using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeClient.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("Username")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
