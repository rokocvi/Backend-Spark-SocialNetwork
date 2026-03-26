namespace Spark.API.DTOs.Spark
{
    public class SparkResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string Content { get; set; } = string.Empty;
        public string[] Tags { get; set; } = Array.Empty<string>();
        public DateOnly SparkDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}