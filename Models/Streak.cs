using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Models
{
    public class Streak
    {
        [Key]
        public int StreakId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public int CurrentStreak { get; set; } // السلسلة الحالية
        public int LongestStreak { get; set; } // أطول سلسلة تم تحقيقها
        public DateTime LastStudyDate { get; set; }

        // العلاقة
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}