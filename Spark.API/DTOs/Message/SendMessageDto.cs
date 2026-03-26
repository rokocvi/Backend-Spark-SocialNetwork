using System.ComponentModel.DataAnnotations;

namespace Spark.API.DTOs.Message
{
    public class SendMessageDto
    {
        [Required]
        public Guid MatchId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
