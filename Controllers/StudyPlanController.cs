using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planner.Data;
using Planner.Services;

namespace Planner.Controllers;

[Authorize]
public class StudyPlanController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly StudyPlannerService _planner;

    public StudyPlanController(ApplicationDbContext db, StudyPlannerService planner)
    {
        _db = db;
        _planner = planner;
    }

    private int StudentId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var plans = await _db.StudyPlans
            .Include(x => x.Subject)
            .Include(x => x.StudyTasks)
            .Where(x => x.StudentId == StudentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        ViewBag.Subjects = await _db.Subjects
            .Where(x => x.StudentId == StudentId)
            .ToListAsync();

        return View(plans);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(int subjectId, int dailyMinutes)
    {
        await _planner.GeneratePlanAsync(
            StudentId,
            subjectId,
            Math.Clamp(dailyMinutes, 15, 600));

        return RedirectToAction(nameof(Index));
    }
}