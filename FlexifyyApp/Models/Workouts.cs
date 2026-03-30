namespace FlexifyyApp.Models
{
    public class Workouts
    {
        public int WorkoutId { get; set; }
        public int UserId { get; set; }     
        public DateTime Date { get; set; }
        public string? WorkoutName { get; set; }
        public string? Notes { get; set; }
        public int? DurationMinutes { get; set; }

        public Users? User { get; set; }
        public ICollection<WorkoutDetails> Details { get; set; }
            = new List<WorkoutDetails>();
    }
}