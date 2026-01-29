using ResumeBuilder.Application.DTOs;

namespace ResumeBuilder.Application.Interfaces;

public interface IResumeService
{
    Task<ResumeDto?> GetResumeByIdAsync(int resumeId);
    Task<ResumeDto?> GetResumeWithDetailsAsync(int resumeId);
    Task<IEnumerable<ResumeDto>> GetResumesByUserIdAsync(string userId);
    Task<ResumeDto?> GetResumeForUserAsync(int resumeId, string userId);
    Task<ResumeDto> CreateResumeAsync(CreateResumeDto dto, string userId);
    Task<ResumeDto> UpdateResumeAsync(UpdateResumeDto dto, string userId);
    Task<bool> DeleteResumeAsync(int resumeId, string userId);
    
    // Education
    Task<EducationDto> AddEducationAsync(EducationDto dto);
    Task<EducationDto> UpdateEducationAsync(EducationDto dto);
    Task<bool> DeleteEducationAsync(int educationId, int resumeId);
    
    // Experience
    Task<ExperienceDto> AddExperienceAsync(ExperienceDto dto);
    Task<ExperienceDto> UpdateExperienceAsync(ExperienceDto dto);
    Task<bool> DeleteExperienceAsync(int experienceId, int resumeId);
    
    // Skills
    Task<SkillDto> AddSkillAsync(SkillDto dto);
    Task<SkillDto> UpdateSkillAsync(SkillDto dto);
    Task<bool> DeleteSkillAsync(int skillId, int resumeId);
    Task<bool> AddSkillsAsync(int resumeId, List<SkillDto> skills);
    
    // Wizard
    Task<ResumeDto> SaveResumeWizardAsync(ResumeWizardDto wizardDto, string userId);
    
    // Dashboard
    Task<DashboardStatsDto> GetDashboardStatsAsync(string userId);
    Task<DashboardStatsDto> GetAdminDashboardStatsAsync();
}
