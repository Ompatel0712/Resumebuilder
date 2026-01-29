using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;

namespace ResumeBuilder.Web.Controllers;

[Authorize]
public class ResumeController : Controller
{
    private readonly IResumeService _resumeService;
    private readonly IPdfGeneratorService _pdfService;
    private readonly IJobMatchingService _matchingService;

    public ResumeController(
        IResumeService resumeService, 
        IPdfGeneratorService pdfService,
        IJobMatchingService matchingService)
    {
        _resumeService = resumeService;
        _pdfService = pdfService;
        _matchingService = matchingService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resumes = await _resumeService.GetResumesByUserIdAsync(userId!);
        return View(resumes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Wizard", new ResumeWizardDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ResumeWizardDto model)
    {
        Console.WriteLine($"[Create] Received POST. Step: {model.CurrentStep}");
        Console.WriteLine($"[Create] Edu Count: {model.Education?.Count ?? 0}, Exp Count: {model.Experience?.Count ?? 0}, Skill Count: {model.Skills?.Count ?? 0}");

        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resume = await _resumeService.SaveResumeWizardAsync(model, userId!);
            
            // Trigger AI matching in background (simulated)
            await _matchingService.CalculateJobMatchesAsync(resume.ResumeId);
            
            return RedirectToAction(nameof(Preview), new { id = resume.ResumeId });
        }

        Console.WriteLine("[Create] ModelState Invalid:");
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine($" - {error.ErrorMessage} ({error.Exception?.Message})");
        }

        // Ensure lists are not null to prevent View crashes or empty states if binding partially failed
        model.Education ??= new List<EducationDto>();
        model.Experience ??= new List<ExperienceDto>();
        model.Skills ??= new List<SkillDto>();

        return View("Wizard", model);
    }

    [HttpGet]
    public async Task<IActionResult> Preview(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resume = await _resumeService.GetResumeForUserAsync(id, userId!);
        if (resume == null) return NotFound();

        ViewBag.Matches = await _matchingService.GetJobMatchesForResumeAsync(id);
        return View(resume);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadPdf(int id, string template = "Modern")
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resume = await _resumeService.GetResumeForUserAsync(id, userId!);
        if (resume == null) return NotFound();

        var pdfBytes = await _pdfService.GenerateResumePdfAsync(resume, template);
        return File(pdfBytes, "application/pdf", $"{resume.FullName.Replace(" ", "_")}_Resume.pdf");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _resumeService.DeleteResumeAsync(id, userId!);
        return RedirectToAction(nameof(Index));
    }
}
