using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.Interfaces;

namespace ResumeBuilder.Web.Controllers;

[Authorize]
public class JobMatchController : Controller
{
    private readonly IJobMatchingService _matchingService;
    private readonly IResumeService _resumeService;

    public JobMatchController(IJobMatchingService matchingService, IResumeService resumeService)
    {
        _matchingService = matchingService;
        _resumeService = resumeService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resumes = await _resumeService.GetResumesByUserIdAsync(userId!);
        return View(resumes);
    }

    public async Task<IActionResult> Matches(int resumeId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resume = await _resumeService.GetResumeForUserAsync(resumeId, userId!);
        if (resume == null) return NotFound();

        var matches = await _matchingService.GetJobMatchesForResumeAsync(resumeId);
        ViewBag.Resume = resume;
        return View(matches);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshMatches(int resumeId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resume = await _resumeService.GetResumeForUserAsync(resumeId, userId!);
        if (resume == null) return NotFound();

        await _matchingService.RefreshAllMatchesForResumeAsync(resumeId);
        return RedirectToAction(nameof(Matches), new { resumeId });
    }
}
