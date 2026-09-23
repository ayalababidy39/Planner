using Planner.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // العلاقات (Navigation Properties)
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
        public ICollection<Point> Points { get; set; } = new List<Point>();
        public ICollection<Streak> Streaks { get; set; } = new List<Streak>();
    }
}
