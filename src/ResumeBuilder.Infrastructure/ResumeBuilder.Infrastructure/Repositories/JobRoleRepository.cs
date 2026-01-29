using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Interfaces;
using ResumeBuilder.Infrastructure.Data;

namespace ResumeBuilder.Infrastructure.Repositories;

public class JobRoleRepository : Repository<JobRole>, IJobRoleRepository
{
    public JobRoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<JobRole>> GetActiveJobRolesAsync()
    {
        return await _context.JobRoles
            .Where(jr => jr.IsActive)
            .OrderBy(jr => jr.RoleName)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobRole>> GetJobRolesByCategoryAsync(string category)
    {
        return await _context.JobRoles
            .Where(jr => jr.IsActive && jr.Category == category)
            .OrderBy(jr => jr.RoleName)
            .ToListAsync();
    }
}
