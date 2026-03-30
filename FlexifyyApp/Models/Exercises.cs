namespace FlexifyyApp.Models
{
    public class Exercises
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MuscleGroup { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsCustom { get; set; } = false;
    }
}