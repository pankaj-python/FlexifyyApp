namespace FlexifyyApp.Models
{
    public class Progress
    {
        public int ProgressId { get; set; }
        public int UserId { get; set; }
        public decimal Weight { get; set; }
        public decimal? BodyFatPercent { get; set; }
        public decimal? ChestCM { get; set; }
        public decimal? WaistCM { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Users? User { get; set; }
    }
}