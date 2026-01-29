namespace ResumeBuilder.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IResumeRepository Resumes { get; }
    IJobRoleRepository JobRoles { get; }
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
