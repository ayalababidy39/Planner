using Planner.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Models
{
    public class StudyPlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalHours { get; set; }

        [StringLength(50)]
        public string Status { get; set; } // مثلاً: Active, Completed, Delayed

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // العلاقات
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }

        public ICollection<StudyTask> StudyTasks { get; set; } = new List<StudyTask>();
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}