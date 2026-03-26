using Spark.API.DTOs.Message;


namespace Spark.API.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponseDto> SendMessage(SendMessageDto dto, Guid senderId);
        Task<List<MessageResponseDto>> GetMessages(Guid matchId, Guid userId);
        Task MarkAsRead(Guid matchId, Guid userId);

    }
}
