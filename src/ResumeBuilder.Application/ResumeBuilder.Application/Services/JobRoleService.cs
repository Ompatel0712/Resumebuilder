using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Interfaces;

namespace ResumeBuilder.Application.Services;

public class JobRoleService : IJobRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public JobRoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<JobRoleDto>> GetAllJobRolesAsync()
    {
        var roles = await _unitOfWork.JobRoles.GetAllAsync();
        return roles.Select(MapToDto).OrderBy(r => r.RoleName);
    }

    public async Task<IEnumerable<JobRoleDto>> GetActiveJobRolesAsync()
    {
        var roles = await _unitOfWork.JobRoles.GetActiveJobRolesAsync();
        return roles.Select(MapToDto);
    }

    public async Task<JobRoleDto?> GetJobRoleByIdAsync(int id)
    {
        var role = await _unitOfWork.JobRoles.GetByIdAsync(id);
        return role == null ? null : MapToDto(role);
    }

    public async Task<JobRoleDto> CreateJobRoleAsync(CreateJobRoleDto dto)
    {
        var jobRole = new JobRole
        {
            RoleName = dto.RoleName,
            Description = dto.Description,
            RequiredSkills = dto.RequiredSkills,
            Category = dto.Category,
            ExperienceLevel = dto.ExperienceLevel,
            MinSalary = dto.MinSalary,
            MaxSalary = dto.MaxSalary,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.JobRoles.AddAsync(jobRole);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(jobRole);
    }

    public async Task<JobRoleDto> UpdateJobRoleAsync(int id, CreateJobRoleDto dto)
    {
        var jobRole = await _unitOfWork.JobRoles.GetByIdAsync(id);
        if (jobRole == null)
            throw new InvalidOperationException("Job role not found");

        jobRole.RoleName = dto.RoleName;
        jobRole.Description = dto.Description;
        jobRole.RequiredSkills = dto.RequiredSkills;
        jobRole.Category = dto.Category;
        jobRole.ExperienceLevel = dto.ExperienceLevel;
        jobRole.MinSalary = dto.MinSalary;
        jobRole.MaxSalary = dto.MaxSalary;
        jobRole.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.JobRoles.Update(jobRole);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(jobRole);
    }

    public async Task<bool> DeleteJobRoleAsync(int id)
    {
        var jobRole = await _unitOfWork.JobRoles.GetByIdAsync(id);
        if (jobRole == null)
            return false;

        _unitOfWork.JobRoles.Remove(jobRole);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleJobRoleStatusAsync(int id)
    {
        var jobRole = await _unitOfWork.JobRoles.GetByIdAsync(id);
        if (jobRole == null)
            return false;

        jobRole.IsActive = !jobRole.IsActive;
        jobRole.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.JobRoles.Update(jobRole);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetAllCategoriesAsync()
    {
        var roles = await _unitOfWork.JobRoles.GetAllAsync();
        return roles
            .Where(r => !string.IsNullOrEmpty(r.Category))
            .Select(r => r.Category!)
            .Distinct()
            .OrderBy(c => c);
    }

    private static JobRoleDto MapToDto(JobRole role)
    {
        return new JobRoleDto
        {
            JobRoleId = role.JobRoleId,
            RoleName = role.RoleName,
            Description = role.Description,
            RequiredSkills = role.RequiredSkills,
            Category = role.Category,
            ExperienceLevel = role.ExperienceLevel,
            MinSalary = role.MinSalary,
            MaxSalary = role.MaxSalary,
            IsActive = role.IsActive
        };
    }
}
