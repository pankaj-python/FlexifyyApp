namespace FlexifyyApp.Models
{
    public class Achievements
    {
        public int AchievementId { get; set; }
        public int UserId { get; set; }
        public string BadgeName { get; set; } = string.Empty;
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
        public Users? User { get; set; }
    }
}