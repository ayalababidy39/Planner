using Planner.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(1, 5)]
        public int Difficulty { get; set; } // من 1 إلى 5

        public DateTime ExamDate { get; set; }
        public int TotalDays { get; set; } // عدد الأيام المتاحة
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // العلاقات
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
    }
}