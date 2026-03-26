using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spark.API.Interfaces;
using System.Security.Claims;

namespace Spark.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
            return Guid.Parse(userIdClaim!);
        }

        // GET api/match
        [HttpGet]
        public async Task<IActionResult> GetMyMatches()
        {
            try
            {
                var userId = GetUserId();
                var result = await _matchService.GetMyMatches(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/match/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatchById(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _matchService.GetMatchById(id, userId);

                if (result == null)
                    return NotFound(new { message = "Match nije pronađen." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/match/run
        [HttpPost("run")]
        public async Task<IActionResult> RunDailyMatching()
        {
            try
            {
                await _matchService.RunDailyMatching();
                return Ok(new { message = "Matching algoritam je pokrenut." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/match/{id}/save
        [HttpPost("{id}/save")]
        public async Task<IActionResult> SaveMatch(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _matchService.SaveMatch(id, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}