using Planner.Models;

namespace Planner.ViewModels
{
    public class DashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int CompletedTasks { get; set; }
        public int TotalTasks { get; set; }
        public int StudyMinutes { get; set; }
        public string? MostDelayedSubject { get; set; }

        public List<StudyTask> TodayTasks { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();
    }
}