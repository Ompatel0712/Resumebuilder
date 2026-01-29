using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;
using System.Security.Claims;

namespace ResumeBuilder.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ResumesController : ControllerBase
{
    private readonly IResumeService _resumeService;

    public ResumesController(IResumeService resumeService)
    {
        _resumeService = resumeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyResumes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var resumes = await _resumeService.GetResumesByUserIdAsync(userId);
        return Ok(resumes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResume(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var resume = await _resumeService.GetResumeForUserAsync(id, userId);
        if (resume == null) return NotFound();

        return Ok(resume);
    }

    [HttpPost]
    public async Task<IActionResult> CreateResume([FromBody] CreateResumeDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var resume = await _resumeService.CreateResumeAsync(model, userId);
        return CreatedAtAction(nameof(GetResume), new { id = resume.ResumeId }, resume);
    }
}
