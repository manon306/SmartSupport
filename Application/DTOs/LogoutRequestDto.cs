using System.ComponentModel.DataAnnotations;
namespace Application.DTOs
{
    public class LogoutRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
