namespace Spark.API.DTOs.Profile
{
    public class ProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }

        public string? ProfileImageBase64 { get; set; }
    }
}
