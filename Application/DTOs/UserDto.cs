namespace Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public IList<string> Roles { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }
}
