using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Spark.API.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("username")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("display_name")]
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("profile_image")]
        public byte[]? ProfileImage { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public ICollection<UserSpark> Sparks { get; set; } = new List<UserSpark>(); 

        public ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
