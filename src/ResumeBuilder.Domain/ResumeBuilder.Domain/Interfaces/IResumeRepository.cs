using ResumeBuilder.Domain.Entities;

namespace ResumeBuilder.Domain.Interfaces;

public interface IResumeRepository : IRepository<Resume>
{
    Task<Resume?> GetResumeWithDetailsAsync(int resumeId);
    Task<IEnumerable<Resume>> GetResumesByUserIdAsync(string userId);
    Task<Resume?> GetResumeWithDetailsForUserAsync(int resumeId, string userId);
}
