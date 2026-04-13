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
    [Route("api/exercise")]
    [Authorize]
    public class ExerciseController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ExerciseController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? muscleGroup)
        {
            var query = _db.Exercises.AsQueryable();

            if (!string.IsNullOrEmpty(muscleGroup))
            {
                query = query.Where(e =>
                    e.MuscleGroup.ToLower() == muscleGroup.ToLower());
            }

            var exercises = await query
                .OrderBy(e => e.MuscleGroup)
                .ThenBy(e => e.Name)
                .Select(e => new ExerciseDto
                {
                    ExerciseId = e.ExerciseId,
                    Name = e.Name,
                    MuscleGroup = e.MuscleGroup,
                    Description = e.Description,
                    VideoUrl = e.VideoUrl,
                    IsCustom = e.IsCustom
                })
                .ToListAsync();

            return Ok(exercises);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exercise = await _db.Exercises
                .FirstOrDefaultAsync(e => e.ExerciseId == id);

            if (exercise == null)
                return NotFound(new { message = "Exercise not found." });

            return Ok(new ExerciseDto
            {
                ExerciseId = exercise.ExerciseId,
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup,
                Description = exercise.Description,
                VideoUrl = exercise.VideoUrl,
                IsCustom = exercise.IsCustom
            });
        }

 
        [HttpPost("custom")]
        public async Task<IActionResult> CreateCustom(
            [FromBody] CreateCustomExerciseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exercise = new Exercises
            {
                Name = dto.Name.Trim(),
                MuscleGroup = dto.MuscleGroup.Trim(),
                Description = dto.Description?.Trim(),
                VideoUrl = null,
                IsCustom = true   
            };

            _db.Exercises.Add(exercise);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Custom exercise created!",
                exerciseId = exercise.ExerciseId,
                name = exercise.Name,
                muscleGroup = exercise.MuscleGroup
            });
        }

        [HttpDelete("custom/{id}")]
        public async Task<IActionResult> DeleteCustom(int id)
        {
            var exercise = await _db.Exercises
                .FirstOrDefaultAsync(e =>
                    e.ExerciseId == id && e.IsCustom == true);

            if (exercise == null)
                return NotFound(new
                {
                    message = "Custom exercise not found. " +
                              "Only custom exercises can be deleted."
                });

            _db.Exercises.Remove(exercise);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Custom exercise deleted successfully!" });
        }
    }
}