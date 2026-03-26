using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spark.API.Models
{
    [Table("matches")]
    public class Match
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("user1_id")]
        public Guid User1Id { get; set; }

        [Required]
        [Column("user2_id")]
        public Guid User2Id { get; set; }

        [Required]
        [Column("spark1_id")]
        public Guid Spark1Id { get; set; }

        [Required]
        [Column("spark2_id")]
        public Guid Spark2Id { get; set; }

        [Column("match_date")]
        public DateOnly MatchDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);

        [Column("user1_saved")]
        public bool User1Saved { get; set; } = false;

        [Column("user2_saved")]
        public bool User2Saved { get; set; } = false;

        [Column("is_permanent")]
        public bool IsPermanent { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      
        [ForeignKey("User1Id")]
        public User User1 { get; set; } = null!;

        [ForeignKey("User2Id")]
        public User User2 { get; set; } = null!;

        [ForeignKey("Spark1Id")]
        public UserSpark Spark1 { get; set; } = null!;

        [ForeignKey("Spark2Id")]
        public UserSpark Spark2 { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}