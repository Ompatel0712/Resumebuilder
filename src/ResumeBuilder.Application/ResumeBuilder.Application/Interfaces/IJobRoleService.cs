using ResumeBuilder.Application.DTOs;

namespace ResumeBuilder.Application.Interfaces;

public interface IJobRoleService
{
    Task<IEnumerable<JobRoleDto>> GetAllJobRolesAsync();
    Task<IEnumerable<JobRoleDto>> GetActiveJobRolesAsync();
    Task<JobRoleDto?> GetJobRoleByIdAsync(int id);
    Task<JobRoleDto> CreateJobRoleAsync(CreateJobRoleDto dto);
    Task<JobRoleDto> UpdateJobRoleAsync(int id, CreateJobRoleDto dto);
    Task<bool> DeleteJobRoleAsync(int id);
    Task<bool> ToggleJobRoleStatusAsync(int id);
    Task<IEnumerable<string>> GetAllCategoriesAsync();
}
