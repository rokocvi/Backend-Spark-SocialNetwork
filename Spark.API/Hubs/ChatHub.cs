using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Spark.API.Data;
using Spark.API.DTOs.Message;
using Spark.API.Interfaces;
using System.Security.Claims;

namespace Spark.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? Context.User?.FindFirst("sub")?.Value;
            return Guid.Parse(userIdClaim!);
        }

        // Korisnik uđe u sobu kad otvori chat
        public async Task JoinMatch(string matchId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, matchId);
        }

        // Korisnik izađe iz sobe
        public async Task LeaveMatch(string matchId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, matchId);
        }

        // Slanje poruke kroz hub
        public async Task SendMessage(string matchId, string content)
        {
            var userId = GetUserId();
            var dto = new SendMessageDto
            {
                MatchId = Guid.Parse(matchId),
                Content = content
            };

            // Spremi u bazu kroz postojeći servis
            var message = await _messageService.SendMessage(dto, userId);

            // Broadcast svima u sobi
            await Clients.Group(matchId).SendAsync("ReceiveMessage", message);
        }

        public async Task Typing(string matchId, string username)
        {
            await Clients.OthersInGroup(matchId).SendAsync("UserTyping", username);
        }

        public async Task StopTyping(string matchId)
        {
            await Clients.OthersInGroup(matchId).SendAsync("UserStoppedTyping");
        }

        public async Task MarkRead(string matchId) 
        {
            var userId = GetUserId();
            await _messageService.MarkAsRead(Guid.Parse(matchId), userId);
            await Clients.OthersInGroup(matchId).SendAsync("MessagesRead"); 
        }

        public async Task JoinUserGroup()
        {
            var userId = GetUserId();
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
    }
}