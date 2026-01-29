using ResumeBuilder.Application.DTOs;

namespace ResumeBuilder.Application.Interfaces;

public interface IJobMatchingService
{
    Task<IEnumerable<JobMatchDto>> CalculateJobMatchesAsync(int resumeId);
    Task<IEnumerable<JobMatchDto>> GetJobMatchesForResumeAsync(int resumeId);
    Task<decimal> CalculateMatchScoreAsync(List<string> userSkills, string requiredSkills);
    Task RefreshAllMatchesForResumeAsync(int resumeId);
}
