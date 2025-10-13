namespace DataLifecycleManager.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repository properties will be added by Infrastructure layer
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
