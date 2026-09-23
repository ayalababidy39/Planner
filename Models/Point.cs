using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Models
{
    public class Point
    {
        [Key]
        public int PointId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public int Points { get; set; } // عدد النقاط (مثلاً +10 أو -5)

        [StringLength(150)]
        public string Reason { get; set; } // سبب النقاط: Complete Task, Daily Goal, etc.

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // العلاقة
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}