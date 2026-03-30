namespace FlexifyyApp.Models
{
    public class WorkoutDetails
    {
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? WeightKG { get; set; }
        public int? RestSeconds { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public Workouts? Workout { get; set; }
        public Exercises? Exercise { get; set; }
    }
}