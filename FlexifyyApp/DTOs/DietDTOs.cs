using System.ComponentModel.DataAnnotations;

namespace FlexifyyApp.DTOs
{
  
    public class CreateDietDto
    {
        [Required]
        [RegularExpression("Breakfast|Lunch|Dinner|Snack",
            ErrorMessage = "MealType must be Breakfast, Lunch, Dinner or Snack")]
        public string MealType { get; set; } = string.Empty;

        [Required]
        public string FoodItems { get; set; } = string.Empty;

        [Range(0, 10000)]
        public int? Calories { get; set; }

        [Range(0, 1000)]
        public int? ProteinG { get; set; }

        [Required]
        public DateTime DietDate { get; set; }

        public bool IsVegetarian { get; set; } = true;
    }


    public class DietDto
    {
        public int DietId { get; set; }
        public string MealType { get; set; } = string.Empty;
        public string FoodItems { get; set; } = string.Empty;
        public int? Calories { get; set; }
        public int? ProteinG { get; set; }
        public DateTime DietDate { get; set; }
        public bool IsVegetarian { get; set; }
    }
}