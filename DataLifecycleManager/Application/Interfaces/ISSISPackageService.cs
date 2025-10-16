using DataLifecycleManager.Domain.Entities;

namespace DataLifecycleManager.Application.Interfaces;

public interface ISSISPackageService : ICrudService<SSISPackage, int>
{
    Task<bool> PackageExistsAsync(string folderName, string projectName, string packageName, int? excludeId = null);
    Task<IEnumerable<DatabaseConnection>> GetAssignedConnectionsAsync(int packageId);
    Task<List<int>> GetAssignedConnectionIdsAsync(int packageId);
    Task<IEnumerable<DatabaseConnection>> GetActiveConnectionsAsync();
    Task AssignConnectionsAsync(int packageId, List<int> connectionIds);
    Task<IEnumerable<SSISPackage>> GetPackagesWithConnectionsAsync();
    Task<IEnumerable<DatabaseConnectionPackage>> GetConnectionPackagesAsync(int packageId);
}
