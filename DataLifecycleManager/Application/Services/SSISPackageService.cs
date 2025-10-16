using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Application.Services;

public class SSISPackageService : CrudService<SSISPackage, int>, ISSISPackageService
{
    private readonly IRepository<SSISPackage, int> _repository;
    private readonly IRepository<DatabaseConnectionPackage, int> _connectionPackageRepository;
    private readonly IRepository<DatabaseConnection, int> _connectionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SSISPackageService> _logger;

    public SSISPackageService(
        IRepository<SSISPackage, int> repository,
        IRepository<DatabaseConnectionPackage, int> connectionPackageRepository,
        IRepository<DatabaseConnection, int> connectionRepository,
        IUnitOfWork unitOfWork,
        ILogger<SSISPackageService> logger)
        : base(repository, unitOfWork)
    {
        _repository = repository;
        _connectionPackageRepository = connectionPackageRepository;
        _connectionRepository = connectionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> PackageExistsAsync(string folderName, string projectName, string packageName, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await AnyAsync(p =>
                p.FolderName == folderName &&
                p.ProjectName == projectName &&
                p.PackageName == packageName &&
                p.Id != excludeId.Value);
        }
        return await AnyAsync(p =>
            p.FolderName == folderName &&
            p.ProjectName == projectName &&
            p.PackageName == packageName);
    }

    public async Task<IEnumerable<DatabaseConnection>> GetAssignedConnectionsAsync(int packageId)
    {
        var connectionPackages = await _connectionPackageRepository.FindAsync(
            dcp => dcp.SSISPackageId == packageId && dcp.IsActive);

        var connectionIds = connectionPackages.Select(cp => cp.DatabaseConnectionId).ToList();
        
        if (!connectionIds.Any())
            return Enumerable.Empty<DatabaseConnection>();

        var connections = await _connectionRepository.FindAsync(c => connectionIds.Contains(c.Id));
        return connections;
    }

    public async Task<List<int>> GetAssignedConnectionIdsAsync(int packageId)
    {
        var connectionPackages = await _connectionPackageRepository.FindAsync(
            dcp => dcp.SSISPackageId == packageId && dcp.IsActive);

        return connectionPackages.Select(cp => cp.DatabaseConnectionId).ToList();
    }

    public async Task<IEnumerable<DatabaseConnection>> GetActiveConnectionsAsync()
    {
        return await _connectionRepository.FindAsync(c => c.Status == ConnectionStatus.Active);
    }

    public async Task AssignConnectionsAsync(int packageId, List<int> connectionIds)
    {
        // Remove existing assignments
        var existingAssignments = await _connectionPackageRepository.FindAsync(
            dcp => dcp.SSISPackageId == packageId);

        await _connectionPackageRepository.DeleteRangeAsync(existingAssignments);

        // Add new assignments
        if (connectionIds != null && connectionIds.Any())
        {
            var newAssignments = connectionIds.Select(connectionId => new DatabaseConnectionPackage
            {
                DatabaseConnectionId = connectionId,
                SSISPackageId = packageId,
                ConnectionRole = "Source",
                IsActive = true
            });

            await _connectionPackageRepository.AddRangeAsync(newAssignments);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<DatabaseConnectionPackage>> GetConnectionPackagesAsync(int packageId)
    {
        return await _connectionPackageRepository.FindAsync(
            dcp => dcp.SSISPackageId == packageId && dcp.IsActive);
    }

    public async Task<IEnumerable<SSISPackage>> GetPackagesWithConnectionsAsync()
    {
        // Get all packages ordered by folder, project, and package name
        var packages = await _repository.GetAllAsync();
        return packages
            .OrderBy(p => p.FolderName)
            .ThenBy(p => p.ProjectName)
            .ThenBy(p => p.PackageName)
            .ToList();
    }
}
