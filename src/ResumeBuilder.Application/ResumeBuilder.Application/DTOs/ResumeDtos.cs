using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.Application.DTOs;

public class ResumeDto
{
    public int ResumeId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<EducationDto> EducationDetails { get; set; } = new();
    public List<ExperienceDto> ExperienceDetails { get; set; } = new();
    public List<SkillDto> Skills { get; set; } = new();
    public List<JobMatchDto> JobMatches { get; set; } = new();
}

public class CreateResumeDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Summary { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone format")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    [MaxLength(500)]
    public string? GitHubUrl { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    [MaxLength(500)]
    public string? PortfolioUrl { get; set; }
}

public class UpdateResumeDto : CreateResumeDto
{
    public int ResumeId { get; set; }
}

public class EducationDto
{
    public int EducationId { get; set; }
    public int ResumeId { get; set; }

    [Required(ErrorMessage = "Institution is required")]
    [MaxLength(300)]
    public string Institution { get; set; } = string.Empty;

    [Required(ErrorMessage = "Degree is required")]
    [MaxLength(200)]
    public string Degree { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FieldOfStudy { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsCurrentlyStudying { get; set; }

    [Range(0, 10.0, ErrorMessage = "GPA must be between 0 and 10.0")]
    public decimal? GPA { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class ExperienceDto
{
    public int ExperienceId { get; set; }
    public int ResumeId { get; set; }

    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(300)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job title is required")]
    [MaxLength(200)]
    public string JobTitle { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(3000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsCurrent { get; set; }
}

public class SkillDto
{
    public int SkillId { get; set; }
    public int ResumeId { get; set; }

    [Required(ErrorMessage = "Skill name is required")]
    [MaxLength(100)]
    public string SkillName { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Proficiency level must be between 1 and 5")]
    public int ProficiencyLevel { get; set; } = 3;

    [MaxLength(50)]
    public string? Category { get; set; }
}

public class JobRoleDto
{
    public int JobRoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RequiredSkills { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ExperienceLevel { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool IsActive { get; set; }
}

public class CreateJobRoleDto
{
    [Required(ErrorMessage = "Role name is required")]
    [MaxLength(200)]
    public string RoleName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Required skills are required")]
    public string RequiredSkills { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Category { get; set; }

    [MaxLength(100)]
    public string? ExperienceLevel { get; set; }

    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
}

public class JobMatchDto
{
    public int MatchId { get; set; }
    public int ResumeId { get; set; }
    public int JobRoleId { get; set; }
    public string JobRoleName { get; set; } = string.Empty;
    public string? JobRoleDescription { get; set; }
    public decimal MatchScore { get; set; }
    public List<string> MatchedSkills { get; set; } = new();
    public List<string> MissingSkills { get; set; } = new();
    public DateTime MatchedAt { get; set; }
}

public class ResumeWizardDto
{
    public int CurrentStep { get; set; } = 1;
    public CreateResumeDto PersonalInfo { get; set; } = new();
    public List<EducationDto> Education { get; set; } = new();
    public List<ExperienceDto> Experience { get; set; } = new();
    public List<SkillDto> Skills { get; set; } = new();
}

public class DashboardStatsDto
{
    public int TotalResumes { get; set; }
    public int TotalJobMatches { get; set; }
    public int TotalUsers { get; set; }
    public decimal AverageMatchScore { get; set; }
    public List<TopJobMatchDto> TopJobMatches { get; set; } = new();
    public List<SkillDistributionDto> SkillDistribution { get; set; } = new();
}

public class TopJobMatchDto
{
    public string JobRoleName { get; set; } = string.Empty;
    public decimal MatchScore { get; set; }
    public string ResumeName { get; set; } = string.Empty;
}

public class SkillDistributionDto
{
    public string SkillName { get; set; } = string.Empty;
    public int Count { get; set; }
}
