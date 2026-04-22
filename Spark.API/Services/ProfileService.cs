using Microsoft.EntityFrameworkCore;
using Spark.API.Data;
using Spark.API.DTOs.Profile;
using Spark.API.Interfaces;

namespace Spark.API.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileDto> GetProfile(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("Korisnik nije pronađen.");

            return MapToDto(user);
        }

        public async Task<ProfileDto> UpdateProfile(Guid userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("Korisnik nije pronađen.");

            if (dto.DisplayName != null)
                user.DisplayName = dto.DisplayName.Trim();

            if (dto.Bio != null)
                user.Bio = dto.Bio.Trim();

            await _context.SaveChangesAsync();
            return MapToDto(user);
        }

        public async Task<ProfileDto> UpdateProfileImage(Guid userId, IFormFile image)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("Korisnik nije pronađen.");

            if (image.Length > 2 * 1024 * 1024)
                throw new Exception("Slika ne smije biti veća od 2MB.");

            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(image.ContentType))
                throw new Exception("Dozvoljeni formati: JPG, PNG, WEBP.");

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            user.ProfileImage = ms.ToArray();

            await _context.SaveChangesAsync();
            return MapToDto(user);
        }

        private static ProfileDto MapToDto(Models.User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            ProfileImageBase64 = user.ProfileImage != null
                ? Convert.ToBase64String(user.ProfileImage)
                : null
        };

        public async Task<ProfileDto> DeleteProfile(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("Korisnik nije pronađen.");

            var dto = MapToDto(user);

            var messages = await _context.Messages
                .Where(m => m.SenderId == userId)
                .ToListAsync();
            _context.Messages.RemoveRange(messages);

            var matches = await _context.Matches
                .Where(m => m.User1Id == userId || m.User2Id == userId)
                .ToListAsync();

            foreach (var match in matches)
            {
                var matchMessages = await _context.Messages
                    .Where(m => m.MatchId == match.Id)
                    .ToListAsync();
                _context.Messages.RemoveRange(matchMessages);
            }
            _context.Matches.RemoveRange(matches);

            var sparks = await _context.Sparks
                .Where(s => s.UserId == userId)
                .ToListAsync();
            _context.Sparks.RemoveRange(sparks);

          
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return dto;
        }
    }
}