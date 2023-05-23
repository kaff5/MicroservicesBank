using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersService.Models
{
    public class User: IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Patronymic { get; set; }
        public bool isBlocked { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
