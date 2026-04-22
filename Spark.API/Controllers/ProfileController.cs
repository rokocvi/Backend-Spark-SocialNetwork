using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spark.API.DTOs.Profile;
using Spark.API.Interfaces;
using System.Security.Claims;

namespace Spark.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET api/profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var result = await _profileService.GetProfile(GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT api/profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var result = await _profileService.UpdateProfile(GetUserId(), dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/profile/image
        [HttpPost("image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile image)
        {
            try
            {
                var result = await _profileService.UpdateProfileImage(GetUserId(), image);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            try
            {
                var result = await _profileService.DeleteProfile(GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{username}")]

        public async Task<IActionResult> GetUserProfile(string username)
        {
            try
            {
                var result = await _profileService.GetUserProfile(username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}