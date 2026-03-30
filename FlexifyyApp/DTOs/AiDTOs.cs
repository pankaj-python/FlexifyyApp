namespace FlexifyyApp.DTOs
{

    public class WorkoutSuggestionDto
    {
        public string Goal { get; set; } = string.Empty;
        public string WorkoutPlan { get; set; } = string.Empty;
        public List<string> Exercises { get; set; } = new();
        public string Tips { get; set; } = string.Empty;
    }

    public class DietSuggestionDto
    {
        public string Goal { get; set; } = string.Empty;
        public List<AiMealDto> Meals { get; set; } = new();
        public int DailyTotalCalories { get; set; }
        public int DailyTotalProtein { get; set; }
    }

    public class AiMealDto
    {
        public string MealType { get; set; } = string.Empty;
        public string FoodItems { get; set; } = string.Empty;
        public int Calories { get; set; }
        public int ProteinG { get; set; }
    }
}
