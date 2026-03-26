using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spark.API.Models
{
    [Table("sparks")]
    public class UserSpark
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Required]
        [Column("content")]
        [MaxLength(280)]
        public string Content { get; set; } = string.Empty;

        [Column("tags")]
        public string[] Tags { get; set; } = Array.Empty<string>();

        [Column("spark_date")]
        public DateOnly SparkDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

    }
}
