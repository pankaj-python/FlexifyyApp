using FlexifyyApp.Data;
using FlexifyyApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlexifyyApp.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UserController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = GetUserId();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new UserProfileDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Height = user.Height,
                Weight = user.Weight,
                Goal = user.Goal,
                CreatedAt = user.CreatedAt
            });
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            user.FullName = dto.FullName.Trim();
            user.Height = dto.Height;
            user.Weight = dto.Weight;
            user.Goal = dto.Goal ?? user.Goal;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully!",
                user = new UserProfileDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Height = user.Height,
                    Weight = user.Weight,
                    Goal = user.Goal,
                    CreatedAt = user.CreatedAt
                }
            });
        }


        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            bool isCurrentValid = BCrypt.Net.BCrypt
                .Verify(dto.CurrentPassword, user.PasswordHash);

            if (!isCurrentValid)
                return BadRequest(new { message = "Current password is incorrect." });

            user.PasswordHash = BCrypt.Net.BCrypt
                .HashPassword(dto.NewPassword, workFactor: 12);

            await _db.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully!" });
        }
    }
}