namespace Spark.API.DTOs.Message
{
    public class MessageResponseDto
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }
}