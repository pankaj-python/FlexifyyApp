using FlexifyyApp.Data;
using FlexifyyApp.DTOs;
using FlexifyyApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlexifyyApp.Controllers
{
    [ApiController]
    [Route("api/progress")]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProgressController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProgressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();

            var progress = new Progress
            {
                UserId = userId,
                Weight = dto.Weight,
                BodyFatPercent = dto.BodyFatPercent,
                ChestCM = dto.ChestCM,
                WaistCM = dto.WaistCM,
                PhotoUrl = dto.PhotoUrl,
                Date = dto.Date,
                CreatedAt = DateTime.UtcNow
            };

            _db.Progress.Add(progress);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Progress logged successfully!",
                progressId = progress.ProgressId
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = GetUserId();

            var entries = await _db.Progress
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Date)
                .Select(p => new ProgressDto
                {
                    ProgressId = p.ProgressId,
                    Weight = p.Weight,
                    BodyFatPercent = p.BodyFatPercent,
                    ChestCM = p.ChestCM,
                    WaistCM = p.WaistCM,
                    PhotoUrl = p.PhotoUrl,
                    Date = p.Date
                })
                .ToListAsync();

            return Ok(entries);
        }


        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            int userId = GetUserId();

            var entries = await _db.Progress
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Date)
                .ToListAsync();

            if (!entries.Any())
                return NotFound(new { message = "No progress entries found." });

            var first = entries.First();
            var latest = entries.Last();
            decimal change = latest.Weight - first.Weight;

            return Ok(new ProgressSummaryDto
            {
                StartWeight = first.Weight,
                CurrentWeight = latest.Weight,
                TotalChange = change,
                TotalEntries = entries.Count
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserId();

            var progress = await _db.Progress
                .FirstOrDefaultAsync(p =>
                    p.ProgressId == id && p.UserId == userId);

            if (progress == null)
                return NotFound(new { message = "Progress entry not found." });

            _db.Progress.Remove(progress);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Progress entry deleted successfully!" });
        }
    }
}