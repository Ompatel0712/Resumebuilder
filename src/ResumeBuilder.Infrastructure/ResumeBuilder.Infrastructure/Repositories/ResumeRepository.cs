using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Interfaces;
using ResumeBuilder.Infrastructure.Data;

namespace ResumeBuilder.Infrastructure.Repositories;

public class ResumeRepository : Repository<Resume>, IResumeRepository
{
    public ResumeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Resume?> GetResumeWithDetailsAsync(int resumeId)
    {
        return await _context.Resumes
            .Include(r => r.EducationDetails.OrderByDescending(e => e.StartDate))
            .Include(r => r.ExperienceDetails.OrderByDescending(e => e.StartDate))
            .Include(r => r.Skills)
            .Include(r => r.JobMatches)
                .ThenInclude(jm => jm.JobRole)
            .FirstOrDefaultAsync(r => r.ResumeId == resumeId);
    }

    public async Task<IEnumerable<Resume>> GetResumesByUserIdAsync(string userId)
    {
        return await _context.Resumes
            .Include(r => r.Skills)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Resume?> GetResumeWithDetailsForUserAsync(int resumeId, string userId)
    {
        return await _context.Resumes
            .Include(r => r.EducationDetails.OrderByDescending(e => e.StartDate))
            .Include(r => r.ExperienceDetails.OrderByDescending(e => e.StartDate))
            .Include(r => r.Skills)
            .Include(r => r.JobMatches)
                .ThenInclude(jm => jm.JobRole)
            .FirstOrDefaultAsync(r => r.ResumeId == resumeId && r.UserId == userId);
    }
}
