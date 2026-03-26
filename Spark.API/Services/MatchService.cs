using Microsoft.EntityFrameworkCore;
using Spark.API.Data;
using Spark.API.DTOs.Match;
using Spark.API.Interfaces;
using Spark.API.Models;

namespace Spark.API.Services
{
    public class MatchService : IMatchService
    {
        private readonly AppDbContext _db;

        public MatchService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<MatchResponseDto>> GetMyMatches(Guid userId)
        {
            var now = DateTime.UtcNow;

            var matches = await _db.Matches
                .Include(m => m.User1)
                .Include(m => m.User2)
                .Include(m => m.Spark1)
                .Include(m => m.Spark2)
                .Where(m =>
                    (m.User1Id == userId || m.User2Id == userId) &&
                    (m.IsPermanent || m.ExpiresAt > now))
                .ToListAsync();

            return matches.Select(m => MapToDto(m, userId)).ToList();
        }

        public async Task<MatchResponseDto?> GetMatchById(Guid matchId, Guid userId)
        {
            var match = await _db.Matches
                .Include(m => m.User1)
                .Include(m => m.User2)
                .Include(m => m.Spark1)
                .Include(m => m.Spark2)
                .FirstOrDefaultAsync(m =>
                    m.Id == matchId &&
                    (m.User1Id == userId || m.User2Id == userId));

            if (match == null) return null;

            return MapToDto(match, userId);
        }

        public async Task RunDailyMatching()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Dohvati sve aktivne Sparkove za danas
            var todaySparks = await _db.Sparks
                .Where(s => s.SparkDate == today && s.IsActive)
                .ToListAsync();

            // Dohvati vec postojece matcheve za danas
            var existingMatches = await _db.Matches
                .Where(m => m.MatchDate == today)
                .Select(m => new { m.User1Id, m.User2Id })
                .ToListAsync();

            var newMatches = new List<Match>();

            // Usporedi svaki Spark sa svakim
            for (int i = 0; i < todaySparks.Count; i++)
            {
                for (int j = i + 1; j < todaySparks.Count; j++)
                {
                    var spark1 = todaySparks[i];
                    var spark2 = todaySparks[j];

                    // Preskoci ako su isti korisnik
                    if (spark1.UserId == spark2.UserId) continue;

                    // Preskoci ako vec postoji match danas
                    var alreadyMatched = existingMatches.Any(m =>
                        (m.User1Id == spark1.UserId && m.User2Id == spark2.UserId) ||
                        (m.User1Id == spark2.UserId && m.User2Id == spark1.UserId));

                    if (alreadyMatched) continue;

                    // Pronadi zajednicke tagove
                    var commonTags = spark1.Tags.Intersect(spark2.Tags).ToArray();

                    // Spoji samo ako imaju barem jedan zajednicki tag
                    if (commonTags.Length > 0)
                    {
                        var match = new Match
                        {
                            User1Id = spark1.UserId,
                            User2Id = spark2.UserId,
                            Spark1Id = spark1.Id,
                            Spark2Id = spark2.Id,
                            MatchDate = today,
                            ExpiresAt = DateTime.UtcNow.AddHours(24)
                        };

                        newMatches.Add(match);
                    }
                }
            }

            if (newMatches.Any())
            {
                _db.Matches.AddRange(newMatches);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<MatchResponseDto> SaveMatch(Guid matchId, Guid userId)
        {
            var match = await _db.Matches
                .Include(m => m.User1)
                .Include(m => m.User2)
                .Include(m => m.Spark1)
                .Include(m => m.Spark2)
                .FirstOrDefaultAsync(m =>
                    m.Id == matchId &&
                    (m.User1Id == userId || m.User2Id == userId));

            if (match == null)
                throw new Exception("Match nije pronađen.");

            // Postavi saved za pravog korisnika
            if (match.User1Id == userId)
                match.User1Saved = true;
            else
                match.User2Saved = true;

            // Ako oboje sačuvaju, postaje permanentan
            if (match.User1Saved && match.User2Saved)
                match.IsPermanent = true;

            await _db.SaveChangesAsync();

            return MapToDto(match, userId);
        }

        // Pomocna metoda za mapiranje
        private MatchResponseDto MapToDto(Match match, Guid currentUserId)
        {
            var isUser1 = match.User1Id == currentUserId;

            var matchedUser = isUser1 ? match.User2 : match.User1;
            var mySpark = isUser1 ? match.Spark1 : match.Spark2;
            var theirSpark = isUser1 ? match.Spark2 : match.Spark1;
            var commonTags = mySpark.Tags.Intersect(theirSpark.Tags).ToArray();

            return new MatchResponseDto
            {
                Id = match.Id,
                MatchedUserId = matchedUser.Id,
                MatchedUsername = matchedUser.Username,
                MatchedDisplayName = matchedUser.DisplayName,
                MySparkContent = mySpark.Content,
                TheirSparkContent = theirSpark.Content,
                CommonTags = commonTags,
                MatchDate = match.MatchDate,
                ExpiresAt = match.ExpiresAt,
                ISaved = isUser1 ? match.User1Saved : match.User2Saved,
                TheySaved = isUser1 ? match.User2Saved : match.User1Saved,
                IsPermanent = match.IsPermanent
            };
        }
    }
}