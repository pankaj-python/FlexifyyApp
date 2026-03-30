namespace FlexifyyApp.Models
{
    public class DietPlans
    {
        public int DietId { get; set; }
        public int UserId { get; set; }
        public string MealType { get; set; } = string.Empty;
        public string FoodItems { get; set; } = string.Empty;
        public int? Calories { get; set; }
        public int? ProteinG { get; set; }
        public DateTime DietDate { get; set; }
        public bool IsVegetarian { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Users? User { get; set; }
    }
}