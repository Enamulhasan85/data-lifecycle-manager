using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Domain.Enums;
using Microsoft.Data.SqlClient;

namespace DataLifecycleManager.Application.Services;

public class DatabaseConnectionService : CrudService<DatabaseConnection, int>, IDatabaseConnectionService
{
    private readonly IRepository<DatabaseConnection, int> _repository;
    private readonly ILogger<DatabaseConnectionService> _logger;

    public DatabaseConnectionService(
        IRepository<DatabaseConnection, int> repository,
        IUnitOfWork unitOfWork,
        ILogger<DatabaseConnectionService> logger)
        : base(repository, unitOfWork)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> TestConnectionAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = await GetByIdAsync(id, cancellationToken);
        if (connection == null)
            return false;

        try
        {
            var connectionString = BuildConnectionString(connection);
            using var sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing connection {ConnectionId}", id);
            return false;
        }
    }

    public async Task<bool> ConnectionNameExistsAsync(string connectionName, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await AnyAsync(c => c.ConnectionName == connectionName && c.Id != excludeId.Value);
        }
        return await AnyAsync(c => c.ConnectionName == connectionName);
    }

    public async Task<IEnumerable<DatabaseConnection>> GetActiveConnectionsAsync()
    {
        return await FindAsync(c => c.Status == ConnectionStatus.Active);
    }

    private string BuildConnectionString(DatabaseConnection connection)
    {
        if (connection.UseWindowsAuthentication)
        {
            return $"Server={connection.ServerAddress};Database={connection.DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        }
        else
        {
            return $"Server={connection.ServerAddress};Database={connection.DatabaseName};User Id={connection.Username};Password={connection.EncryptedPassword};TrustServerCertificate=true;";
        }
    }
}
