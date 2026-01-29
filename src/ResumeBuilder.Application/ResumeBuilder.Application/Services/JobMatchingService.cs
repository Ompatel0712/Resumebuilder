using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Interfaces;

namespace ResumeBuilder.Application.Services;

public class JobMatchingService : IJobMatchingService
{
    private readonly IUnitOfWork _unitOfWork;

    public JobMatchingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<JobMatchDto>> CalculateJobMatchesAsync(int resumeId)
    {
        var resume = await _unitOfWork.Resumes.GetResumeWithDetailsAsync(resumeId);
        if (resume == null)
            return Enumerable.Empty<JobMatchDto>();

        var userSkills = resume.Skills != null 
            ? resume.Skills.Select(s => s.SkillName.ToLower().Trim()).ToList()
            : new List<string>();
        var jobRoles = await _unitOfWork.JobRoles.GetActiveJobRolesAsync();

        var matches = new List<JobMatchDto>();
        var matchRepo = _unitOfWork.Repository<ResumeJobMatch>();

        foreach (var jobRole in jobRoles)
        {
             // Step 1: Normalize Skills
            var requiredSkills = jobRole.RequiredSkills
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.ToLower().Trim())
                .ToList();

            // Step 2: Find Matches
            var matchedSkills = userSkills.Intersect(requiredSkills).ToList();
            var missingSkills = requiredSkills.Except(userSkills).ToList();

            // Step 3: Calculate Base Score (Percentage of skills matched)
            decimal matchScore = 0;
            if (requiredSkills.Count > 0)
            {
                matchScore = (decimal)matchedSkills.Count / requiredSkills.Count * 100;
            }

            // Step 4: Add Bonuses
            // Bonus 1: Proficiency (Better skills = higher score)
            decimal proficiencyBonus = 0;
            if (resume.Skills != null)
            {
                proficiencyBonus = CalculateProficiencyBonus(resume.Skills, matchedSkills);
            }
            
            // Bonus 2: Experience (More years = higher score)
            decimal experienceBonus = 0;
            if (resume.ExperienceDetails != null)
            {
                experienceBonus = CalculateExperienceBonus(resume.ExperienceDetails, jobRole.ExperienceLevel);
            }

            // Final Calculation: Base + P-Bonus + E-Bonus (Max 100)
            matchScore = matchScore + proficiencyBonus + experienceBonus;
            if (matchScore > 100) matchScore = 100; // Cap at 100%
            
            matchScore = Math.Round(matchScore, 2);

            // Step 5: Save or Update Match
            var existingMatch = await matchRepo.FirstOrDefaultAsync(
                m => m.ResumeId == resumeId && m.JobRoleId == jobRole.JobRoleId);

            if (existingMatch != null)
            {
                // Update existing
                existingMatch.MatchScore = matchScore;
                existingMatch.MatchedSkills = string.Join(",", matchedSkills);
                existingMatch.MissingSkills = string.Join(",", missingSkills);
                existingMatch.MatchedAt = DateTime.UtcNow;
                matchRepo.Update(existingMatch);
            }
            else
            {
                // Create new
                var newMatch = new ResumeJobMatch
                {
                    ResumeId = resumeId,
                    JobRoleId = jobRole.JobRoleId,
                    MatchScore = matchScore,
                    MatchedSkills = string.Join(",", matchedSkills),
                    MissingSkills = string.Join(",", missingSkills),
                    MatchedAt = DateTime.UtcNow
                };
                await matchRepo.AddAsync(newMatch);
            }

            // Add to result list
            matches.Add(new JobMatchDto
            {
                ResumeId = resumeId,
                JobRoleId = jobRole.JobRoleId,
                JobRoleName = jobRole.RoleName,
                JobRoleDescription = jobRole.Description,
                MatchScore = matchScore,
                MatchedSkills = matchedSkills.Select(s => CapitalizeFirst(s)).ToList(),
                MissingSkills = missingSkills.Select(s => CapitalizeFirst(s)).ToList(),
                MatchedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync();

        return matches.OrderByDescending(m => m.MatchScore);
    }

    public async Task<IEnumerable<JobMatchDto>> GetJobMatchesForResumeAsync(int resumeId)
    {
        var matchRepo = _unitOfWork.Repository<ResumeJobMatch>();
        var matches = await matchRepo.FindAsync(m => m.ResumeId == resumeId);

        var result = new List<JobMatchDto>();
        foreach (var match in matches)
        {
            var jobRole = await _unitOfWork.JobRoles.GetByIdAsync(match.JobRoleId);
            result.Add(new JobMatchDto
            {
                MatchId = match.MatchId,
                ResumeId = match.ResumeId,
                JobRoleId = match.JobRoleId,
                JobRoleName = jobRole?.RoleName ?? "Unknown",
                JobRoleDescription = jobRole?.Description,
                MatchScore = match.MatchScore,
                MatchedSkills = match.MatchedSkills?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => CapitalizeFirst(s.Trim())).ToList() ?? new List<string>(),
                MissingSkills = match.MissingSkills?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => CapitalizeFirst(s.Trim())).ToList() ?? new List<string>(),
                MatchedAt = match.MatchedAt
            });
        }

        return result.OrderByDescending(m => m.MatchScore);
    }

    public async Task<decimal> CalculateMatchScoreAsync(List<string> userSkills, string requiredSkills)
    {
        var required = requiredSkills
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.ToLower().Trim())
            .ToList();

        var userSkillsLower = userSkills.Select(s => s.ToLower().Trim()).ToList();
        var matchedCount = userSkillsLower.Intersect(required).Count();

        return required.Count > 0
            ? Math.Round((decimal)matchedCount / required.Count * 100, 2)
            : 0;
    }

    public async Task RefreshAllMatchesForResumeAsync(int resumeId)
    {
        await CalculateJobMatchesAsync(resumeId);
    }

    private decimal CalculateProficiencyBonus(ICollection<Skill> skills, List<string> matchedSkills)
    {
        if (!matchedSkills.Any()) return 0;

        var matchedProficiencies = skills
            .Where(s => matchedSkills.Contains(s.SkillName.ToLower()))
            .Select(s => s.ProficiencyLevel);

        if (!matchedProficiencies.Any()) return 0;

        var avgProficiency = matchedProficiencies.Average();
        // Max bonus of 5% for expert level (5) proficiency
        return (decimal)(avgProficiency - 3) * 1.5m;
    }

    private decimal CalculateExperienceBonus(ICollection<ExperienceDetail> experiences, string? requiredLevel)
    {
        if (string.IsNullOrEmpty(requiredLevel) || experiences == null || !experiences.Any())
            return 0;

        var totalYears = experiences.Sum(e =>
        {
            var endDate = e.IsCurrent ? DateTime.UtcNow : (e.EndDate ?? DateTime.UtcNow);
            return (endDate - e.StartDate).TotalDays / 365.0;
        });

        var expectedYears = requiredLevel.ToLower() switch
        {
            "entry" => 1,
            "mid" => 3,
            "senior" => 5,
            "lead" => 8,
            _ => 3
        };

        // Bonus/penalty based on experience match
        var difference = totalYears - expectedYears;
        return (decimal)Math.Min(Math.Max(difference * 2, -10), 5);
    }

    private static string CapitalizeFirst(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToUpper(input[0]) + input[1..];
    }
}
