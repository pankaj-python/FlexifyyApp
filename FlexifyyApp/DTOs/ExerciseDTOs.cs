using System.ComponentModel.DataAnnotations;

namespace FlexifyyApp.DTOs
{

    public class ExerciseDto
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MuscleGroup { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsCustom { get; set; }
    }


    public class CreateCustomExerciseDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MuscleGroup { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}