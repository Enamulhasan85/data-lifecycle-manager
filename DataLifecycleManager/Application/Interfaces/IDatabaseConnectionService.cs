using DataLifecycleManager.Domain.Entities;

namespace DataLifecycleManager.Application.Interfaces;

public interface IDatabaseConnectionService : ICrudService<DatabaseConnection, int>
{
    Task<bool> TestConnectionAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ConnectionNameExistsAsync(string connectionName, int? excludeId = null);
    Task<IEnumerable<DatabaseConnection>> GetActiveConnectionsAsync();
}
