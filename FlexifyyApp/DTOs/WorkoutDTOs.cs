using System.ComponentModel.DataAnnotations;

namespace FlexifyyApp.DTOs
{

    public class WorkoutDetailDto
    {
        public int ExerciseId { get; set; }
        public string? ExerciseName { get; set; }
        public string? MuscleGroup { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? WeightKG { get; set; }
        public int? RestSeconds { get; set; }
    }

    public class CreateWorkoutDto
    {
        [Required]
        public DateTime Date { get; set; }

        [StringLength(100)]
        public string? WorkoutName { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? DurationMinutes { get; set; }

        [Required]
        public List<CreateWorkoutDetailDto> Details { get; set; } = new();
    }


    public class CreateWorkoutDetailDto
    {
        [Required]
        public int ExerciseId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Sets { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Reps { get; set; }

        public decimal? WeightKG { get; set; }
        public int? RestSeconds { get; set; }
    }


    public class WorkoutSummaryDto
    {
        public int WorkoutId { get; set; }
        public DateTime Date { get; set; }
        public string? WorkoutName { get; set; }
        public int? DurationMinutes { get; set; }
        public int TotalExercises { get; set; }
    }

    public class WorkoutDetailResponseDto
    {
        public int WorkoutId { get; set; }
        public DateTime Date { get; set; }
        public string? WorkoutName { get; set; }
        public string? Notes { get; set; }
        public int? DurationMinutes { get; set; }
        public List<WorkoutDetailDto> Details { get; set; } = new();
    }

    public class UpdateWorkoutDto
    {
        [StringLength(100)]
        public string? WorkoutName { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? DurationMinutes { get; set; }

        public List<CreateWorkoutDetailDto> Details { get; set; } = new();
    }
}