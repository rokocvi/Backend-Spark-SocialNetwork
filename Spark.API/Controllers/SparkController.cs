using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spark.API.DTOs.Spark;
using Spark.API.Interfaces;
using System.Security.Claims;

namespace Spark.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SparkController : ControllerBase
    {
        private readonly ISparkService _sparkService;

        public SparkController(ISparkService sparkService)
        {
            _sparkService = sparkService;
        }

        // Pomocna metoda - dohvati userId iz JWT tokena
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
            return Guid.Parse(userIdClaim!);
        }

        // POST api/spark
        [HttpPost]
        public async Task<IActionResult> CreateSpark([FromBody] CreateSparkDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _sparkService.CreateSpark(dto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/spark/today
        [HttpGet("today")]
        public async Task<IActionResult> GetTodaySpark()
        {
            try
            {
                var userId = GetUserId();
                var result = await _sparkService.GetTodaySpark(userId);

                if (result == null)
                    return NotFound(new { message = "Nisi još objavio Spark za danas." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/spark/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTodaySparks()
        {
            try
            {
                var result = await _sparkService.GetAllTodaySparks();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE api/spark/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpark(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _sparkService.DeleteSpark(id, userId);

                if (!result)
                    return NotFound(new { message = "Spark nije pronađen." });

                return Ok(new { message = "Spark je deaktiviran." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //GET api/spark/history
        [HttpGet("history")]
        public async Task<IActionResult> GetSparkHistory()
        {
            try
            {
                var userId = GetUserId();
                var result = await _sparkService.GetSparkHistory(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}