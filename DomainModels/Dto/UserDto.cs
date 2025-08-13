namespace DomainModels.Dto
{
    public class UserDto
    {
        public required string Email { get; set; } = string.Empty;
        public required string Username { get; set; }
        public required string Password { get; set; } = string.Empty;
    }
}
