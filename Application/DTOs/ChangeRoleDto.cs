using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ChangeRoleDto
    {
        [Required]
        public string Role { get; set; } = null!;
    }
}
