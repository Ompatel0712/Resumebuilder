using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Interfaces;

namespace ResumeBuilder.Application.Services;

public class ResumeService : IResumeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJobMatchingService _jobMatchingService;

    public ResumeService(IUnitOfWork unitOfWork, IJobMatchingService jobMatchingService)
    {
        _unitOfWork = unitOfWork;
        _jobMatchingService = jobMatchingService;
    }

    public async Task<ResumeDto?> GetResumeByIdAsync(int resumeId)
    {
        var resume = await _unitOfWork.Resumes.GetByIdAsync(resumeId);
        return resume == null ? null : MapToDto(resume);
    }

    public async Task<ResumeDto?> GetResumeWithDetailsAsync(int resumeId)
    {
        var resume = await _unitOfWork.Resumes.GetResumeWithDetailsAsync(resumeId);
        return resume == null ? null : MapToDto(resume);
    }

    public async Task<IEnumerable<ResumeDto>> GetResumesByUserIdAsync(string userId)
    {
        var resumes = await _unitOfWork.Resumes.GetResumesByUserIdAsync(userId);
        return resumes.Select(MapToDto);
    }

    public async Task<ResumeDto?> GetResumeForUserAsync(int resumeId, string userId)
    {
        var resume = await _unitOfWork.Resumes.GetResumeWithDetailsForUserAsync(resumeId, userId);
        return resume == null ? null : MapToDto(resume);
    }

    public async Task<ResumeDto> CreateResumeAsync(CreateResumeDto dto, string userId)
    {
        var resume = new Resume
        {
            UserId = userId,
            Title = dto.Title,
            Summary = dto.Summary,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            LinkedInUrl = dto.LinkedInUrl,
            GitHubUrl = dto.GitHubUrl,
            PortfolioUrl = dto.PortfolioUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Resumes.AddAsync(resume);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(resume);
    }

    public async Task<ResumeDto> UpdateResumeAsync(UpdateResumeDto dto, string userId)
    {
        var resume = await _unitOfWork.Resumes.FirstOrDefaultAsync(r => r.ResumeId == dto.ResumeId && r.UserId == userId);
        if (resume == null)
            throw new InvalidOperationException("Resume not found or access denied");

        resume.Title = dto.Title;
        resume.Summary = dto.Summary;
        resume.FullName = dto.FullName;
        resume.Email = dto.Email;
        resume.Phone = dto.Phone;
        resume.Address = dto.Address;
        resume.LinkedInUrl = dto.LinkedInUrl;
        resume.GitHubUrl = dto.GitHubUrl;
        resume.PortfolioUrl = dto.PortfolioUrl;
        resume.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Resumes.Update(resume);
        await _unitOfWork.SaveChangesAsync();

        // Refresh job matches when resume is updated
        await _jobMatchingService.RefreshAllMatchesForResumeAsync(resume.ResumeId);

        return MapToDto(resume);
    }

    public async Task<bool> DeleteResumeAsync(int resumeId, string userId)
    {
        var resume = await _unitOfWork.Resumes.FirstOrDefaultAsync(r => r.ResumeId == resumeId && r.UserId == userId);
        if (resume == null)
            return false;

        _unitOfWork.Resumes.Remove(resume);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<EducationDto> AddEducationAsync(EducationDto dto)
    {
        var education = new EducationDetail
        {
            ResumeId = dto.ResumeId,
            Institution = dto.Institution,
            Degree = dto.Degree,
            FieldOfStudy = dto.FieldOfStudy,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCurrentlyStudying = dto.IsCurrentlyStudying,
            GPA = dto.GPA,
            Description = dto.Description
        };

        await _unitOfWork.Repository<EducationDetail>().AddAsync(education);
        await _unitOfWork.SaveChangesAsync();

        dto.EducationId = education.EducationId;
        return dto;
    }

    public async Task<EducationDto> UpdateEducationAsync(EducationDto dto)
    {
        var education = await _unitOfWork.Repository<EducationDetail>().GetByIdAsync(dto.EducationId);
        if (education == null)
            throw new InvalidOperationException("Education record not found");

        education.Institution = dto.Institution;
        education.Degree = dto.Degree;
        education.FieldOfStudy = dto.FieldOfStudy;
        education.StartDate = dto.StartDate;
        education.EndDate = dto.EndDate;
        education.IsCurrentlyStudying = dto.IsCurrentlyStudying;
        education.GPA = dto.GPA;
        education.Description = dto.Description;

        _unitOfWork.Repository<EducationDetail>().Update(education);
        await _unitOfWork.SaveChangesAsync();

        return dto;
    }

    public async Task<bool> DeleteEducationAsync(int educationId, int resumeId)
    {
        var education = await _unitOfWork.Repository<EducationDetail>()
            .FirstOrDefaultAsync(e => e.EducationId == educationId && e.ResumeId == resumeId);
        
        if (education == null)
            return false;

        _unitOfWork.Repository<EducationDetail>().Remove(education);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<ExperienceDto> AddExperienceAsync(ExperienceDto dto)
    {
        var experience = new ExperienceDetail
        {
            ResumeId = dto.ResumeId,
            CompanyName = dto.CompanyName,
            JobTitle = dto.JobTitle,
            Location = dto.Location,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCurrent = dto.IsCurrent
        };

        await _unitOfWork.Repository<ExperienceDetail>().AddAsync(experience);
        await _unitOfWork.SaveChangesAsync();

        dto.ExperienceId = experience.ExperienceId;
        return dto;
    }

    public async Task<ExperienceDto> UpdateExperienceAsync(ExperienceDto dto)
    {
        var experience = await _unitOfWork.Repository<ExperienceDetail>().GetByIdAsync(dto.ExperienceId);
        if (experience == null)
            throw new InvalidOperationException("Experience record not found");

        experience.CompanyName = dto.CompanyName;
        experience.JobTitle = dto.JobTitle;
        experience.Location = dto.Location;
        experience.Description = dto.Description;
        experience.StartDate = dto.StartDate;
        experience.EndDate = dto.EndDate;
        experience.IsCurrent = dto.IsCurrent;

        _unitOfWork.Repository<ExperienceDetail>().Update(experience);
        await _unitOfWork.SaveChangesAsync();

        return dto;
    }

    public async Task<bool> DeleteExperienceAsync(int experienceId, int resumeId)
    {
        var experience = await _unitOfWork.Repository<ExperienceDetail>()
            .FirstOrDefaultAsync(e => e.ExperienceId == experienceId && e.ResumeId == resumeId);
        
        if (experience == null)
            return false;

        _unitOfWork.Repository<ExperienceDetail>().Remove(experience);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<SkillDto> AddSkillAsync(SkillDto dto)
    {
        var skill = new Skill
        {
            ResumeId = dto.ResumeId,
            SkillName = dto.SkillName,
            ProficiencyLevel = dto.ProficiencyLevel,
            Category = dto.Category
        };

        await _unitOfWork.Repository<Skill>().AddAsync(skill);
        await _unitOfWork.SaveChangesAsync();

        // Refresh job matches when skills change
        await _jobMatchingService.RefreshAllMatchesForResumeAsync(dto.ResumeId);

        dto.SkillId = skill.SkillId;
        return dto;
    }

    public async Task<SkillDto> UpdateSkillAsync(SkillDto dto)
    {
        var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(dto.SkillId);
        if (skill == null)
            throw new InvalidOperationException("Skill not found");

        skill.SkillName = dto.SkillName;
        skill.ProficiencyLevel = dto.ProficiencyLevel;
        skill.Category = dto.Category;

        _unitOfWork.Repository<Skill>().Update(skill);
        await _unitOfWork.SaveChangesAsync();

        // Refresh job matches when skills change
        await _jobMatchingService.RefreshAllMatchesForResumeAsync(dto.ResumeId);

        return dto;
    }

    public async Task<bool> DeleteSkillAsync(int skillId, int resumeId)
    {
        var skill = await _unitOfWork.Repository<Skill>()
            .FirstOrDefaultAsync(s => s.SkillId == skillId && s.ResumeId == resumeId);
        
        if (skill == null)
            return false;

        _unitOfWork.Repository<Skill>().Remove(skill);
        await _unitOfWork.SaveChangesAsync();

        // Refresh job matches when skills change
        await _jobMatchingService.RefreshAllMatchesForResumeAsync(resumeId);

        return true;
    }

    public async Task<bool> AddSkillsAsync(int resumeId, List<SkillDto> skills)
    {
        var skillEntities = skills.Select(s => new Skill
        {
            ResumeId = resumeId,
            SkillName = s.SkillName,
            ProficiencyLevel = s.ProficiencyLevel,
            Category = s.Category
        });

        await _unitOfWork.Repository<Skill>().AddRangeAsync(skillEntities);
        await _unitOfWork.SaveChangesAsync();

        // Refresh job matches
        await _jobMatchingService.RefreshAllMatchesForResumeAsync(resumeId);

        return true;
    }

    public async Task<ResumeDto> SaveResumeWizardAsync(ResumeWizardDto wizardDto, string userId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Create resume
            var resume = new Resume
            {
                UserId = userId,
                Title = wizardDto.PersonalInfo.Title,
                Summary = wizardDto.PersonalInfo.Summary,
                FullName = wizardDto.PersonalInfo.FullName,
                Email = wizardDto.PersonalInfo.Email,
                Phone = wizardDto.PersonalInfo.Phone,
                Address = wizardDto.PersonalInfo.Address,
                LinkedInUrl = wizardDto.PersonalInfo.LinkedInUrl,
                GitHubUrl = wizardDto.PersonalInfo.GitHubUrl,
                PortfolioUrl = wizardDto.PersonalInfo.PortfolioUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Resumes.AddAsync(resume);
            await _unitOfWork.SaveChangesAsync();

            // Add education
            foreach (var edu in wizardDto.Education)
            {
                var education = new EducationDetail
                {
                    ResumeId = resume.ResumeId,
                    Institution = edu.Institution,
                    Degree = edu.Degree,
                    FieldOfStudy = edu.FieldOfStudy,
                    StartDate = edu.StartDate,
                    EndDate = edu.EndDate,
                    IsCurrentlyStudying = edu.IsCurrentlyStudying,
                    GPA = edu.GPA,
                    Description = edu.Description
                };
                await _unitOfWork.Repository<EducationDetail>().AddAsync(education);
            }

            // Add experience
            foreach (var exp in wizardDto.Experience)
            {
                var experience = new ExperienceDetail
                {
                    ResumeId = resume.ResumeId,
                    CompanyName = exp.CompanyName,
                    JobTitle = exp.JobTitle,
                    Location = exp.Location,
                    Description = exp.Description,
                    StartDate = exp.StartDate,
                    EndDate = exp.EndDate,
                    IsCurrent = exp.IsCurrent
                };
                await _unitOfWork.Repository<ExperienceDetail>().AddAsync(experience);
            }

            // Add skills
            foreach (var skill in wizardDto.Skills)
            {
                var skillEntity = new Skill
                {
                    ResumeId = resume.ResumeId,
                    SkillName = skill.SkillName,
                    ProficiencyLevel = skill.ProficiencyLevel,
                    Category = skill.Category
                };
                await _unitOfWork.Repository<Skill>().AddAsync(skillEntity);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Calculate job matches for new resume
            await _jobMatchingService.CalculateJobMatchesAsync(resume.ResumeId);

            return await GetResumeWithDetailsAsync(resume.ResumeId) ?? MapToDto(resume);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId)
    {
        // 1. Get raw data
        var resumes = await _unitOfWork.Resumes.GetResumesByUserIdAsync(userId);
        var resumeList = resumes.ToList();
        
        // 2. Collect all skills for stats
        var allSkills = new List<Skill>();
        foreach (var r in resumeList)
        {
            if (r.Skills != null)
                allSkills.AddRange(r.Skills);
        }

        // 3. Get Job Matches
        var matchRepo = _unitOfWork.Repository<ResumeJobMatch>();
        var resumeIds = resumeList.Select(r => r.ResumeId).ToList();
        var matches = await matchRepo.FindAsync(m => resumeIds.Contains(m.ResumeId));
        var matchList = matches.ToList();

        // 4. Calculate Top Matches (Logic Breakdown)
        var topMatches = matchList
            .OrderByDescending(m => m.MatchScore)
            .Take(5)
            .Select(m => new TopJobMatchDto
            {
                JobRoleName = m.JobRole?.RoleName ?? "Unknown",
                MatchScore = m.MatchScore,
                ResumeName = resumeList.FirstOrDefault(r => r.ResumeId == m.ResumeId)?.Title ?? "Current Resume"
            })
            .ToList();

        // 5. Calculate Skill Distribution (Logic Breakdown)
        var skillStats = allSkills
            .GroupBy(s => s.SkillName)
            .Select(group => new SkillDistributionDto 
            { 
                SkillName = group.Key, 
                Count = group.Count() 
            })
            .OrderByDescending(s => s.Count)
            .Take(10)
            .ToList();

        // 6. Return Result
        return new DashboardStatsDto
        {
            TotalResumes = resumeList.Count,
            TotalJobMatches = matchList.Count,
            AverageMatchScore = matchList.Any() ? matchList.Average(m => m.MatchScore) : 0,
            TopJobMatches = topMatches,
            SkillDistribution = skillStats
        };
    }

    public async Task<DashboardStatsDto> GetAdminDashboardStatsAsync()
    {
        var allResumes = await _unitOfWork.Resumes.GetAllAsync();
        var resumeList = allResumes.ToList();
        
        var matchRepo = _unitOfWork.Repository<ResumeJobMatch>();
        var allMatches = await matchRepo.GetAllAsync();
        var matchList = allMatches.ToList();

        var skillRepo = _unitOfWork.Repository<Skill>();
        var allSkills = await skillRepo.GetAllAsync();
        var skillList = allSkills.ToList();

        return new DashboardStatsDto
        {
            TotalResumes = resumeList.Count,
            TotalJobMatches = matchList.Count,
            AverageMatchScore = matchList.Any() ? matchList.Average(m => m.MatchScore) : 0,
            TopJobMatches = matchList
                .OrderByDescending(m => m.MatchScore)
                .Take(10)
                .Select(m => new TopJobMatchDto
                {
                    JobRoleName = m.JobRole?.RoleName ?? "Unknown",
                    MatchScore = m.MatchScore,
                    ResumeName = resumeList.FirstOrDefault(r => r.ResumeId == m.ResumeId)?.Title ?? "Unknown"
                })
                .ToList(),
            SkillDistribution = skillList
                .GroupBy(s => s.SkillName)
                .Select(g => new SkillDistributionDto { SkillName = g.Key, Count = g.Count() })
                .OrderByDescending(s => s.Count)
                .Take(15)
                .ToList()
        };
    }

    private static ResumeDto MapToDto(Resume resume)
    {
        // 1. Map Basic Info
        var dto = new ResumeDto
        {
            ResumeId = resume.ResumeId,
            UserId = resume.UserId,
            Title = resume.Title,
            Summary = resume.Summary,
            FullName = resume.FullName,
            Email = resume.Email,
            Phone = resume.Phone,
            Address = resume.Address,
            LinkedInUrl = resume.LinkedInUrl,
            GitHubUrl = resume.GitHubUrl,
            PortfolioUrl = resume.PortfolioUrl,
            CreatedAt = resume.CreatedAt,
            UpdatedAt = resume.UpdatedAt
        };

        // 2. Map Education (if exists)
        if (resume.EducationDetails != null)
        {
            foreach (var e in resume.EducationDetails)
            {
                dto.EducationDetails.Add(new EducationDto
                {
                    EducationId = e.EducationId,
                    ResumeId = e.ResumeId,
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrentlyStudying = e.IsCurrentlyStudying,
                    GPA = e.GPA,
                    Description = e.Description
                });
            }
        }

        // 3. Map Experience (if exists)
        if (resume.ExperienceDetails != null)
        {
            foreach (var e in resume.ExperienceDetails)
            {
                dto.ExperienceDetails.Add(new ExperienceDto
                {
                    ExperienceId = e.ExperienceId,
                    ResumeId = e.ResumeId,
                    CompanyName = e.CompanyName,
                    JobTitle = e.JobTitle,
                    Location = e.Location,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrent = e.IsCurrent
                });
            }
        }

        // 4. Map Skills (if exists)
        if (resume.Skills != null)
        {
            foreach (var s in resume.Skills)
            {
                dto.Skills.Add(new SkillDto
                {
                    SkillId = s.SkillId,
                    ResumeId = s.ResumeId,
                    SkillName = s.SkillName,
                    ProficiencyLevel = s.ProficiencyLevel,
                    Category = s.Category
                });
            }
        }

        // 5. Map Job Matches (if exists)
        if (resume.JobMatches != null)
        {
            foreach (var m in resume.JobMatches)
            {
                dto.JobMatches.Add(new JobMatchDto
                {
                    MatchId = m.MatchId,
                    ResumeId = m.ResumeId,
                    JobRoleId = m.JobRoleId,
                    JobRoleName = m.JobRole?.RoleName ?? "Unknown Role",
                    JobRoleDescription = m.JobRole?.Description,
                    MatchScore = m.MatchScore,
                    MatchedAt = m.MatchedAt,
                    // Convert Comma-Separated Strings to Lists
                    MatchedSkills = string.IsNullOrEmpty(m.MatchedSkills) 
                        ? new List<string>() 
                        : m.MatchedSkills.Split(',').ToList(),
                    MissingSkills = string.IsNullOrEmpty(m.MissingSkills) 
                        ? new List<string>() 
                        : m.MissingSkills.Split(',').ToList()
                });
            }
        }

        return dto;
    }
}
