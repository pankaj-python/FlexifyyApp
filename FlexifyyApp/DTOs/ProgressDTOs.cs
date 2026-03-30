using System.ComponentModel.DataAnnotations;

namespace FlexifyyApp.DTOs
{

    public class CreateProgressDto
    {
        [Required]
        [Range(1, 500, ErrorMessage = "Weight must be between 1 and 500 kg")]
        public decimal Weight { get; set; }

        [Range(0, 100)]
        public decimal? BodyFatPercent { get; set; }

        public decimal? ChestCM { get; set; }
        public decimal? WaistCM { get; set; }
        public string? PhotoUrl { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }

    public class ProgressDto
    {
        public int ProgressId { get; set; }
        public decimal Weight { get; set; }
        public decimal? BodyFatPercent { get; set; }
        public decimal? ChestCM { get; set; }
        public decimal? WaistCM { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime Date { get; set; }
    }

    public class ProgressSummaryDto
    {
        public decimal StartWeight { get; set; }
        public decimal CurrentWeight { get; set; }
        public decimal TotalChange { get; set; }
        public int TotalEntries { get; set; }
    }
}