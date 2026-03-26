using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spark.API.DTOs.Message;
using Spark.API.Interfaces;
using System.Security.Claims;

namespace Spark.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
            return Guid.Parse(userIdClaim!);
        }

        // POST api/message
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _messageService.SendMessage(dto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/message/{matchId}
        [HttpGet("{matchId}")]
        public async Task<IActionResult> GetMessages(Guid matchId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _messageService.GetMessages(matchId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT api/message/{matchId}/read
        [HttpPut("{matchId}/read")]
        public async Task<IActionResult> MarkAsRead(Guid matchId)
        {
            try
            {
                var userId = GetUserId();
                await _messageService.MarkAsRead(matchId, userId);
                return Ok(new { message = "Poruke označene kao pročitane." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}