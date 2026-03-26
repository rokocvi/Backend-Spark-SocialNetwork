namespace Spark.API.DTOs.Match
{
    public class MatchResponseDto
    {
        public Guid Id { get; set; }
        public Guid MatchedUserId { get; set; }
        public string MatchedUsername { get; set; } = string.Empty;
        public string? MatchedDisplayName { get; set; }
        public string MySparkContent { get; set; } = string.Empty;
        public string TheirSparkContent { get; set; } = string.Empty;
        public string[] CommonTags { get; set; } = Array.Empty<string>();
        public DateOnly MatchDate { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool ISaved { get; set; }
        public bool TheySaved { get; set; }
        public bool IsPermanent { get; set; }
    }
}