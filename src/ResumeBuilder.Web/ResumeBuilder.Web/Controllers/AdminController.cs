using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;

namespace ResumeBuilder.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IJobRoleService _jobRoleService;
    private readonly IResumeService _resumeService;

    public AdminController(IJobRoleService jobRoleService, IResumeService resumeService)
    {
        _jobRoleService = jobRoleService;
        _resumeService = resumeService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _resumeService.GetDashboardStatsAsync(null); // System-wide stats for admin
        var jobRoles = await _jobRoleService.GetAllJobRolesAsync();
        ViewBag.JobRolesCount = jobRoles.Count();
        return View(stats);
    }

    public async Task<IActionResult> JobRoles()
    {
        var roles = await _jobRoleService.GetAllJobRolesAsync();
        return View(roles);
    }

    [HttpGet]
    public IActionResult CreateJobRole()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJobRole(CreateJobRoleDto model)
    {
        if (ModelState.IsValid)
        {
            await _jobRoleService.CreateJobRoleAsync(model);
            return RedirectToAction(nameof(JobRoles));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditJobRole(int id)
    {
        var role = await _jobRoleService.GetJobRoleByIdAsync(id);
        if (role == null) return NotFound();
        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditJobRole(int id, CreateJobRoleDto model)
    {
        if (ModelState.IsValid)
        {
            await _jobRoleService.UpdateJobRoleAsync(id, model);
            return RedirectToAction(nameof(JobRoles));
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleJobRoleStatus(int id)
    {
        await _jobRoleService.ToggleJobRoleStatusAsync(id);
        return RedirectToAction(nameof(JobRoles));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJobRole(int id)
    {
        await _jobRoleService.DeleteJobRoleAsync(id);
        return RedirectToAction(nameof(JobRoles));
    }
}
