using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.Interfaces;

namespace ResumeBuilder.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IResumeService _resumeService;

    public DashboardController(IResumeService resumeService)
    {
        _resumeService = resumeService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var stats = await _resumeService.GetDashboardStatsAsync(userId);
        var recentResumes = await _resumeService.GetResumesByUserIdAsync(userId);
        
        ViewBag.Resumes = recentResumes;
        return View(stats);
    }
}
