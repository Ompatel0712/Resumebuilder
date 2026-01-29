using ResumeBuilder.Domain.Entities;

namespace ResumeBuilder.Domain.Interfaces;

public interface IJobRoleRepository : IRepository<JobRole>
{
    Task<IEnumerable<JobRole>> GetActiveJobRolesAsync();
    Task<IEnumerable<JobRole>> GetJobRolesByCategoryAsync(string category);
}
