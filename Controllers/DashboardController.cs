using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planner.Data;
using Planner.ViewModels;

namespace Planner.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var tasks = await _db.StudyTasks
            .Include(x => x.StudyPlan)
            .ThenInclude(x => x.Subject)
            .Where(x => x.StudyPlan.StudentId == studentId)
            .OrderBy(x => x.TaskDate)
            .ToListAsync();

        var subjects = await _db.Subjects
            .Where(x => x.StudentId == studentId)
            .OrderBy(x => x.ExamDate)
            .ToListAsync();

        var streak = await _db.Streaks.FirstOrDefaultAsync(x => x.StudentId == studentId);

        var points = await _db.Points
            .Where(x => x.StudentId == studentId)
            .SumAsync(x => (int?)x.Points) ?? 0;

        var delayedSubject = tasks
            .Where(x => x.Status != "Completed" && x.TaskDate.Date < DateTime.Today)
            .GroupBy(x => x.StudyPlan.Subject.Name)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        var vm = new DashboardViewModel
        {
            StudentName = User.Identity?.Name ?? "Student",
            TotalPoints = points,
            CurrentStreak = streak?.CurrentStreak ?? 0,
            LongestStreak = streak?.LongestStreak ?? 0,
            CompletedTasks = tasks.Count(x => x.Status == "Completed"),
            TotalTasks = tasks.Count,
            StudyMinutes = tasks.Sum(x => x.ActualMinutes ?? 0),
            MostDelayedSubject = delayedSubject,
            TodayTasks = tasks.Where(x => x.TaskDate.Date == DateTime.Today).Take(10).ToList(),
            Subjects = subjects
        };

        return View(vm);
    }
}