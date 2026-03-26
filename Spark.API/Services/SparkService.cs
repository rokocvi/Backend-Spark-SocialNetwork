using Microsoft.EntityFrameworkCore;
using Spark.API.Data;
using Spark.API.DTOs.Spark;
using Spark.API.Interfaces;
using Spark.API.Models;

namespace Spark.API.Services
{
    public class SparkService : ISparkService
    {
        private readonly AppDbContext _db;

        public SparkService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SparkResponseDto> CreateSpark(CreateSparkDto dto, Guid userId)
        {
            // Provjeri ima li korisnik vec Spark za danas
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existingSpark = await _db.Sparks
                .FirstOrDefaultAsync(s => s.UserId == userId && s.SparkDate == today);

            if (existingSpark != null)
                throw new Exception("Vec si objavio Spark za danas.");

            var spark = new UserSpark
            {
                UserId = userId,
                Content = dto.Content,
                Tags = dto.Tags,
                SparkDate = today
            };

            _db.Sparks.Add(spark);
            await _db.SaveChangesAsync();

            // Dohvati korisnika za response
            var user = await _db.Users.FindAsync(userId);

            return MapToDto(spark, user!);
        }

        public async Task<SparkResponseDto?> GetTodaySpark(Guid userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var spark = await _db.Sparks
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.SparkDate == today && s.IsActive);

            if (spark == null) return null;

            return MapToDto(spark, spark.User);
        }

        public async Task<List<SparkResponseDto>> GetAllTodaySparks()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var sparks = await _db.Sparks
                .Include(s => s.User)
                .Where(s => s.SparkDate == today && s.IsActive)
                .ToListAsync();

            return sparks.Select(s => MapToDto(s, s.User)).ToList();
        }

        public async Task<bool> DeleteSpark(Guid sparkId, Guid userId)
        {
            var spark = await _db.Sparks
                .FirstOrDefaultAsync(s => s.Id == sparkId && s.UserId == userId);

            if (spark == null) return false;

            // Umjesto brisanja, deaktiviraj
            spark.IsActive = false;
            await _db.SaveChangesAsync();

            return true;
        }

        // Pomocna metoda za mapiranje modela u DTO
        private SparkResponseDto MapToDto(UserSpark spark, User user)
        {
            return new SparkResponseDto
            {
                Id = spark.Id,
                UserId = spark.UserId,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Content = spark.Content,
                Tags = spark.Tags,
                SparkDate = spark.SparkDate,
                IsActive = spark.IsActive,
                CreatedAt = spark.CreatedAt
            };
        }
    }
}