using FlexifyyApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlexifyyApp.Controllers
{
    [ApiController]
    [Route("api/achievements")]
    [Authorize]
    public class AchievementController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AchievementController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = GetUserId();

            var achievements = await _db.Achievements
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.EarnedAt)
                .Select(a => new
                {
                    a.AchievementId,
                    a.BadgeName,
                    a.EarnedAt
                })
                .ToListAsync();

            return Ok(achievements);
        }
    }
}