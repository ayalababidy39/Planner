using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planner.Data;
using Planner.Models;

namespace Planner.Controllers;

[Authorize]
public class SubjectsController : Controller
{
    private readonly ApplicationDbContext _db;

    public SubjectsController(ApplicationDbContext db) => _db = db;

    private int StudentId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
        => View(await _db.Subjects
            .Where(x => x.StudentId == StudentId)
            .OrderBy(x => x.ExamDate)
            .ToListAsync());

    [HttpGet]
    public IActionResult Create() => View(new Subject { ExamDate = DateTime.Today.AddDays(7) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Subject subject)
    {
        ModelState.Remove(nameof(subject.Student));
        ModelState.Remove(nameof(subject.StudyPlans));

        if (!ModelState.IsValid) return View(subject);

        subject.StudentId = StudentId;
        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var subject = await _db.Subjects
            .FirstOrDefaultAsync(x => x.SubjectId == id && x.StudentId == StudentId);

        return subject == null ? NotFound() : View(subject);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Subject subject)
    {
        if (id != subject.SubjectId) return NotFound();

        ModelState.Remove(nameof(subject.Student));
        ModelState.Remove(nameof(subject.StudyPlans));

        if (!ModelState.IsValid) return View(subject);

        var existing = await _db.Subjects
            .FirstOrDefaultAsync(x => x.SubjectId == id && x.StudentId == StudentId);

        if (existing == null) return NotFound();

        existing.Name = subject.Name;
        existing.Difficulty = subject.Difficulty;
        existing.ExamDate = subject.ExamDate;
        existing.TotalDays = subject.TotalDays;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var subject = await _db.Subjects
            .FirstOrDefaultAsync(x => x.SubjectId == id && x.StudentId == StudentId);

        if (subject != null)
        {
            _db.Subjects.Remove(subject);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}