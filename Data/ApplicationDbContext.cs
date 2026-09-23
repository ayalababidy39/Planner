using Microsoft.EntityFrameworkCore;
using Planner.Models; // تأكد أن هذا هو الـ namespace الصحيح للموديلات عندك

namespace Planner.Data // تأكد أن هذا هو الـ namespace الصحيح للمشروع عندك
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // الجداول
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudyPlan> StudyPlans { get; set; }
        public DbSet<StudyTask> StudyTasks { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Streak> Streaks { get; set; }
        public DbSet<Progress> Progresses { get; set; }

        // الإعدادات لمنع خطأ الـ Cascade
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // هذا الكود يمر على كل العلاقات في قاعدة البيانات
            // ويجبر جميع الحذف التتابعي على أن يكون Restrict (بدون حذف تلقائي)
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}