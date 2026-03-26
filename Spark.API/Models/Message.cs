using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Spark.API.Models
{
    [Table("messages")]
    public class Message
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("match_id")]
        public Guid MatchId { get; set; }

        [Required]
        [Column("sender_id")]
        public Guid SenderId { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        [Column("sent_at")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("MatchId")]
        public Match Match { get; set; } = null!;

        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

    }
}
