using FlexifyyApp.Data;
using FlexifyyApp.DTOs;
using FlexifyyApp.Helper;
using FlexifyyApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexifyyApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtHelper _jwt;

        public AuthController(AppDbContext db, JwtHelper jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool emailExists = await _db.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (emailExists)
                return Conflict(new { message = "Email already registered." });

            string hashedPassword = BCrypt.Net.BCrypt
                .HashPassword(dto.Password, workFactor: 12);

            var user = new Users
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = hashedPassword,
                Height = dto.Height,
                Weight = dto.Weight,
                Goal = dto.Goal ?? "Maintain",
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            string token = _jwt.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,   
                FullName = user.FullName,
                Email = user.Email,
                Message = "Registration successful!"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _db.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == dto.Email.ToLower());

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            bool isPasswordValid = BCrypt.Net.BCrypt
                .Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid email or password." });

            string token = _jwt.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,   
                FullName = user.FullName,
                Email = user.Email,
                Message = "Login successful!"
            });
        }
    }
}