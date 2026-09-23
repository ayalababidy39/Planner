using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Models
{
    public class Progress
    {
        [Key]
        public int ProgressId { get; set; }

        [Required]
        public int PlanId { get; set; }

        public DateTime Date { get; set; }
        public double StudyTime { get; set; } // الوقت المدروس في هذا اليوم (بالساعات أو الدقائق حسب اختيارك)

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        // العلاقة
        [ForeignKey("PlanId")]
        public StudyPlan StudyPlan { get; set; }
    }
}