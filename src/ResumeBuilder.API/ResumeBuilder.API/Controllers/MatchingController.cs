using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.Interfaces;

namespace ResumeBuilder.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MatchingController : ControllerBase
{
    private readonly IJobMatchingService _jobMatchingService;

    public MatchingController(IJobMatchingService jobMatchingService)
    {
        _jobMatchingService = jobMatchingService;
    }

    [HttpGet("{resumeId}")]
    public async Task<IActionResult> GetMatches(int resumeId)
    {
        var matches = await _jobMatchingService.GetJobMatchesForResumeAsync(resumeId);
        return Ok(matches);
    }

    [HttpPost("{resumeId}/refresh")]
    public async Task<IActionResult> RefreshMatches(int resumeId)
    {
        await _jobMatchingService.RefreshAllMatchesForResumeAsync(resumeId);
        return Ok(new { message = "Matches refreshed successfully" });
    }
}
