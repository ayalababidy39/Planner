using Microsoft.EntityFrameworkCore;
using Planner.Data;
using Planner.Models;

namespace Planner.Services
{
    public class StudyPlannerService
    {
        private readonly ApplicationDbContext _db;

        public StudyPlannerService(ApplicationDbContext db) => _db = db;

        /// <summary>
        /// توليد خطة دراسية لطالب بناءً على المادة وعدد الدقائق اليومية.
        /// </summary>
        public async Task<StudyPlan> GeneratePlanAsync(int studentId, int subjectId, int dailyMinutes)
        {
            var subject = await _db.Subjects
                .FirstOrDefaultAsync(x => x.SubjectId == subjectId && x.StudentId == studentId);

            if (subject == null)
                throw new InvalidOperationException("Subject not found for this student.");

            // احذف الخطة القديمة (غير المكتملة) لنفس المادة إن وجدت
            var existingPlans = await _db.StudyPlans
                .Include(x => x.StudyTasks)
                .Where(x => x.StudentId == studentId && x.SubjectId == subjectId)
                .ToListAsync();

            foreach (var old in existingPlans)
            {
                _db.StudyTasks.RemoveRange(old.StudyTasks);
                _db.StudyPlans.Remove(old);
            }

            var startDate = DateTime.Today;
            var endDate = subject.ExamDate.Date;

            if (endDate <= startDate)
                throw new InvalidOperationException("Exam date must be in the future.");

            var totalDays = (endDate - startDate).Days;
            if (totalDays <= 0) totalDays = 1;

            // صعوبة المادة تؤثر على توزيع الوقت
            var difficultyFactor = 1.0 + (subject.Difficulty - 1) * 0.15;
            var totalMinutesAvailable = dailyMinutes * totalDays;
            var adjustedMinutes = (int)(totalMinutesAvailable * difficultyFactor);

            var plan = new StudyPlan
            {
                StudentId = studentId,
                SubjectId = subjectId,
                StartDate = startDate,
                EndDate = endDate,
                TotalHours = adjustedMinutes / 60.0,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            // أنشئ مهمة لكل يوم
            var perDayMinutes = Math.Max(15, dailyMinutes);
            for (int i = 0; i < totalDays; i++)
            {
                var taskDate = startDate.AddDays(i);

                // قسّم اليوم إلى جلسة أو جلستين
                var sessions = perDayMinutes >= 120 ? 2 : 1;
                var minutesPerSession = perDayMinutes / sessions;

                for (int s = 0; s < sessions; s++)
                {
                    plan.StudyTasks.Add(new StudyTask
                    {
                        Title = $"{subject.Name} - Session {s + 1}",
                        TaskDate = taskDate,
                        EstimatedMinutes = minutesPerSession,
                        Status = "Pending"
                    });
                }
            }

            _db.StudyPlans.Add(plan);
            await _db.SaveChangesAsync();

            return plan;
        }

        /// <summary>
        /// إكمال مهمة، مع إضافة نقاط وتحديث الـ Streak.
        /// </summary>
        public async Task CompleteTaskAsync(int studentId, int taskId)
        {
            var task = await _db.StudyTasks
                .Include(x => x.StudyPlan)
                .FirstOrDefaultAsync(x => x.TaskId == taskId && x.StudyPlan.StudentId == studentId);

            if (task == null || task.Status == "Completed")
                return;

            task.Status = "Completed";
            task.CompletedAt = DateTime.Now;
            task.ActualMinutes ??= task.EstimatedMinutes;

            // أضف نقاط
            var points = task.EstimatedMinutes >= 60 ? 15 : 10;
            _db.Points.Add(new Point
            {
                StudentId = studentId,
                Points = points,
                Reason = "Complete Task",
                CreatedAt = DateTime.Now
            });

            // حدّث الـ Streak
            var streak = await _db.Streaks.FirstOrDefaultAsync(x => x.StudentId == studentId);
            if (streak == null)
            {
                streak = new Streak
                {
                    StudentId = studentId,
                    CurrentStreak = 1,
                    LongestStreak = 1,
                    LastStudyDate = DateTime.Today
                };
                _db.Streaks.Add(streak);
            }
            else
            {
                var today = DateTime.Today;
                var last = streak.LastStudyDate.Date;

                if (last == today)
                {
                    // نفس اليوم، لا تغيير
                }
                else if (last == today.AddDays(-1))
                {
                    streak.CurrentStreak += 1;
                }
                else
                {
                    streak.CurrentStreak = 1;
                }

                streak.LastStudyDate = today;
                if (streak.CurrentStreak > streak.LongestStreak)
                    streak.LongestStreak = streak.CurrentStreak;
            }

            // حدّث حالة الخطة إذا اكتملت كل مهامها
            var plan = task.StudyPlan;
            var allTasks = await _db.StudyTasks
                .Where(x => x.PlanId == plan.PlanId)
                .ToListAsync();

            if (allTasks.All(x => x.Status == "Completed"))
            {
                plan.Status = "Completed";
            }

            await _db.SaveChangesAsync();
        }
    }
}