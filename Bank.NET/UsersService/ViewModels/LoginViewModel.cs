using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UsersService.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        public string? RedirectUrl { get; set; }
    }
}
