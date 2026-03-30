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
    [Route("api/workout")]
    [Authorize]
    public class WorkoutController : ControllerBase
    {
        private readonly AppDbContext _db;

        public WorkoutController(AppDbContext db)
        {
            _db = db;
        }


        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkoutDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();

            var workout = new Workouts
            {
                UserId = userId,
                Date = dto.Date,
                WorkoutName = dto.WorkoutName?.Trim(),
                Notes = dto.Notes?.Trim(),
                DurationMinutes = dto.DurationMinutes
            };

            _db.Workouts.Add(workout);
            await _db.SaveChangesAsync();


            foreach (var detail in dto.Details)
            {
                var workoutDetail = new WorkoutDetails
                {
                    WorkoutId = workout.WorkoutId,
                    ExerciseId = detail.ExerciseId,
                    Sets = detail.Sets,
                    Reps = detail.Reps,
                    WeightKG = detail.WeightKG,
                    RestSeconds = detail.RestSeconds
                };
                _db.WorkoutDetails.Add(workoutDetail);
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Workout created successfully!",
                workoutId = workout.WorkoutId
            });
        }


        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            int userId = GetUserId();

            var workouts = await _db.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Select(w => new WorkoutSummaryDto
                {
                    WorkoutId = w.WorkoutId,
                    Date = w.Date,
                    WorkoutName = w.WorkoutName,
                    DurationMinutes = w.DurationMinutes,
                    TotalExercises = w.Details.Count
                })
                .ToListAsync();

            return Ok(workouts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int userId = GetUserId();

            var workout = await _db.Workouts
                .Include(w => w.Details)
                    .ThenInclude(d => d.Exercise)
                .FirstOrDefaultAsync(w =>
                    w.WorkoutId == id && w.UserId == userId);

            if (workout == null)
                return NotFound(new { message = "Workout not found." });

            return Ok(new WorkoutDetailResponseDto
            {
                WorkoutId = workout.WorkoutId,
                Date = workout.Date,
                WorkoutName = workout.WorkoutName,
                Notes = workout.Notes,
                DurationMinutes = workout.DurationMinutes,
                Details = workout.Details.Select(d => new WorkoutDetailDto
                {
                    ExerciseId = d.ExerciseId,
                    ExerciseName = d.Exercise?.Name,
                    MuscleGroup = d.Exercise?.MuscleGroup,
                    Sets = d.Sets,
                    Reps = d.Reps,
                    WeightKG = d.WeightKG,
                    RestSeconds = d.RestSeconds
                }).ToList()
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateWorkoutDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();

            var workout = await _db.Workouts
                .Include(w => w.Details)
                .FirstOrDefaultAsync(w =>
                    w.WorkoutId == id && w.UserId == userId);

            if (workout == null)
                return NotFound(new { message = "Workout not found." });

            workout.WorkoutName = dto.WorkoutName?.Trim() ?? workout.WorkoutName;
            workout.Notes = dto.Notes?.Trim() ?? workout.Notes;
            workout.DurationMinutes = dto.DurationMinutes ?? workout.DurationMinutes;

            _db.WorkoutDetails.RemoveRange(workout.Details);


            foreach (var detail in dto.Details)
            {
                _db.WorkoutDetails.Add(new WorkoutDetails
                {
                    WorkoutId = workout.WorkoutId,
                    ExerciseId = detail.ExerciseId,
                    Sets = detail.Sets,
                    Reps = detail.Reps,
                    WeightKG = detail.WeightKG,
                    RestSeconds = detail.RestSeconds
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new { message = "Workout updated successfully!" });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserId();

            var workout = await _db.Workouts
                .FirstOrDefaultAsync(w =>
                    w.WorkoutId == id && w.UserId == userId);

            if (workout == null)
                return NotFound(new { message = "Workout not found." });

            _db.Workouts.Remove(workout);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Workout deleted successfully!" });
        }
    }
}