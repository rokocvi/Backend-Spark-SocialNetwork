using System.ComponentModel.DataAnnotations;

namespace Spark.API.DTOs.Spark
{
    public class CreateSparkDto
    {
        [Required]
        [MaxLength(200)]
        public string Content { get; set; } = string.Empty;

        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}
