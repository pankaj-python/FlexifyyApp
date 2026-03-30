using FlexifyyApp.Data;
using FlexifyyApp.DTOs;
using FlexifyyApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace FlexifyyApp.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly GeminiService _gemini;

        public AiController(AppDbContext db, GeminiService gemini)
        {
            _db = db;
            _gemini = gemini;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }


        [HttpGet("workout-suggestion")]
        public async Task<IActionResult> GetWorkoutSuggestion()
        {
            int userId = GetUserId();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            var prompt = $@"You are a professional fitness trainer.
Create a personalized workout plan for this user:
- Name: {user.FullName}
- Goal: {user.Goal ?? "General Fitness"}
- Weight: {user.Weight ?? 70} kg
- Height: {user.Height ?? 170} cm

Respond in this EXACT JSON format only, no extra text, no markdown:
{{
  ""workoutPlan"": ""Brief description of the plan"",
  ""exercises"": [""Exercise 1 - sets x reps"", ""Exercise 2 - sets x reps"", ""Exercise 3 - sets x reps"", ""Exercise 4 - sets x reps"", ""Exercise 5 - sets x reps""],
  ""tips"": ""One important tip for this user""
}}";

            try
            {
                var aiResponse = await _gemini.AskGemini(prompt);

             
                var cleaned = aiResponse
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var parsed = JsonSerializer.Deserialize<JsonElement>(cleaned);

                return Ok(new WorkoutSuggestionDto
                {
                    Goal = user.Goal ?? "General Fitness",
                    WorkoutPlan = parsed.GetProperty("workoutPlan").GetString() ?? "",
                    Exercises = parsed.GetProperty("exercises")
                                       .EnumerateArray()
                                       .Select(e => e.GetString() ?? "")
                                       .ToList(),
                    Tips = parsed.GetProperty("tips").GetString() ?? ""
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "AI error.", error = ex.Message });
            }
        }


        [HttpGet("diet-suggestion")]
        public async Task<IActionResult> GetDietSuggestion()
        {
            int userId = GetUserId();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            var prompt = $@"You are a professional nutritionist specializing in vegetarian Indian diet.
Create a personalized vegetarian meal plan for:
- Name: {user.FullName}
- Goal: {user.Goal ?? "Maintain"}
- Weight: {user.Weight ?? 70} kg

Respond in this EXACT JSON format only, no extra text, no markdown:
{{
  ""meals"": [
    {{""mealType"": ""Breakfast"", ""foodItems"": ""food items"", ""calories"": 400, ""proteinG"": 20}},
    {{""mealType"": ""Lunch"", ""foodItems"": ""food items"", ""calories"": 600, ""proteinG"": 30}},
    {{""mealType"": ""Dinner"", ""foodItems"": ""food items"", ""calories"": 500, ""proteinG"": 25}},
    {{""mealType"": ""Snack"", ""foodItems"": ""food items"", ""calories"": 200, ""proteinG"": 10}}
  ],
  ""dailyTotalCalories"": 1700,
  ""dailyTotalProtein"": 85
}}";

            try
            {
                var aiResponse = await _gemini.AskGemini(prompt);

                var cleaned = aiResponse
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var parsed = JsonSerializer.Deserialize<JsonElement>(cleaned);

                var meals = parsed.GetProperty("meals")
                    .EnumerateArray()
                    .Select(m => new AiMealDto
                    {
                        MealType = m.GetProperty("mealType").GetString() ?? "",
                        FoodItems = m.GetProperty("foodItems").GetString() ?? "",
                        Calories = m.GetProperty("calories").GetInt32(),
                        ProteinG = m.GetProperty("proteinG").GetInt32()
                    }).ToList();

                return Ok(new DietSuggestionDto
                {
                    Goal = user.Goal ?? "Maintain",
                    Meals = meals,
                    DailyTotalCalories = parsed.GetProperty("dailyTotalCalories").GetInt32(),
                    DailyTotalProtein = parsed.GetProperty("dailyTotalProtein").GetInt32()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "AI error.", error = ex.Message });
            }
        }
    }
}