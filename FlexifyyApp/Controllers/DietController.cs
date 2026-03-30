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
    [Route("api/diet")]
    [Authorize]
    public class DietController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DietController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetToday()
        {
            int userId = GetUserId();
            var today = DateTime.UtcNow.Date;

            var meals = await _db.DietPlans
                .Where(d => d.UserId == userId &&
                            d.DietDate.Date == today)
                .Select(d => new DietDto
                {
                    DietId = d.DietId,
                    MealType = d.MealType,
                    FoodItems = d.FoodItems,
                    Calories = d.Calories,
                    ProteinG = d.ProteinG,
                    DietDate = d.DietDate,
                    IsVegetarian = d.IsVegetarian
                })
                .ToListAsync();

            return Ok(meals);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDietDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();

            var diet = new DietPlans
            {
                UserId = userId,
                MealType = dto.MealType,
                FoodItems = dto.FoodItems.Trim(),
                Calories = dto.Calories,
                ProteinG = dto.ProteinG,
                DietDate = dto.DietDate,
                IsVegetarian = dto.IsVegetarian,
                CreatedAt = DateTime.UtcNow
            };

            _db.DietPlans.Add(diet);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Diet entry added successfully!",
                dietId = diet.DietId
            });
        }


        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            int userId = GetUserId();

            var query = _db.DietPlans
                .Where(d => d.UserId == userId);

            if (from.HasValue)
                query = query.Where(d => d.DietDate >= from.Value);

            if (to.HasValue)
                query = query.Where(d => d.DietDate <= to.Value);

            var meals = await query
                .OrderByDescending(d => d.DietDate)
                .Select(d => new DietDto
                {
                    DietId = d.DietId,
                    MealType = d.MealType,
                    FoodItems = d.FoodItems,
                    Calories = d.Calories,
                    ProteinG = d.ProteinG,
                    DietDate = d.DietDate,
                    IsVegetarian = d.IsVegetarian
                })
                .ToListAsync();

            return Ok(meals);
        }
    }
}