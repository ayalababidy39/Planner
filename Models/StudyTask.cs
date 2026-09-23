using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Models
{
    public class StudyTask
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } // مثل: OOP ساعتين

        public DateTime TaskDate { get; set; }

        public int EstimatedMinutes { get; set; } // الوقت المتوقع بالدقائق
        public int? ActualMinutes { get; set; } // الوقت الفعلي (يقبل null إذا لم تنجز بعد)

        [StringLength(50)]
        public string Status { get; set; } // Pending, Completed, Skipped

        public DateTime? CompletedAt { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        // العلاقة
        [ForeignKey("PlanId")]
        public StudyPlan StudyPlan { get; set; }
    }
}