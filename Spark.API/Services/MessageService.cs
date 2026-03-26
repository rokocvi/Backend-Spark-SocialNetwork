using Microsoft.EntityFrameworkCore;
using Spark.API.Data;
using Spark.API.DTOs.Message;
using Spark.API.Interfaces;
using Spark.API.Models;

namespace Spark.API.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _db;

        public MessageService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MessageResponseDto> SendMessage(SendMessageDto dto, Guid senderId)
        {
            // Provjeri postoji li match i je li korisnik dio tog matcha
            var match = await _db.Matches
                .FirstOrDefaultAsync(m =>
                    m.Id == dto.MatchId &&
                    (m.User1Id == senderId || m.User2Id == senderId));

            if (match == null)
                throw new Exception("Match nije pronađen.");

            // Provjeri nije li match istekao
            if (!match.IsPermanent && match.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Match je istekao, ne možeš slati poruke.");

            var message = new Message
            {
                MatchId = dto.MatchId,
                SenderId = senderId,
                Content = dto.Content
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            // Dohvati podatke o posiljatelju
            var sender = await _db.Users.FindAsync(senderId);

            return MapToDto(message, sender!);
        }

        public async Task<List<MessageResponseDto>> GetMessages(Guid matchId, Guid userId)
        {
            // Provjeri je li korisnik dio matcha
            var match = await _db.Matches
                .FirstOrDefaultAsync(m =>
                    m.Id == matchId &&
                    (m.User1Id == userId || m.User2Id == userId));

            if (match == null)
                throw new Exception("Match nije pronađen.");

            var messages = await _db.Messages
                .Include(m => m.Sender)
                .Where(m => m.MatchId == matchId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return messages.Select(m => MapToDto(m, m.Sender)).ToList();
        }

        public async Task MarkAsRead(Guid matchId, Guid userId)
        {
            // Oznaci sve neprocitane poruke kao procitane
            // (samo poruke koje NIJE poslao trenutni korisnik)
            var unreadMessages = await _db.Messages
                .Where(m =>
                    m.MatchId == matchId &&
                    m.SenderId != userId &&
                    m.IsRead == false)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            await _db.SaveChangesAsync();
        }

        private MessageResponseDto MapToDto(Message message, User sender)
        {
            return new MessageResponseDto
            {
                Id = message.Id,
                MatchId = message.MatchId,
                SenderId = message.SenderId,
                SenderUsername = sender.Username,
                Content = message.Content,
                IsRead = message.IsRead,
                SentAt = message.SentAt
            };
        }
    }
}