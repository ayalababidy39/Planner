using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Services;

namespace Planner.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly StudyPlannerService _planner;

    public TasksController(StudyPlannerService planner) => _planner = planner;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _planner.CompleteTaskAsync(studentId, id);
        return RedirectToAction("Index", "Dashboard");
    }
}