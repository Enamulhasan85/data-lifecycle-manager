using DataLifecycleManager.Application.DTOs.SSISPackage;
using DataLifecycleManager.Domain.Entities;

namespace DataLifecycleManager.Application.Interfaces;

public interface ISSISPackageService : ICrudService<SSISPackage, int>
{
    Task<(bool Success, PackageExistenceResultDto Result)> TestPackageConnectionAsync(int packageId);
    Task<(bool Success, CatalogExecutionResultDto Result)> ExecutePackageAsync(int packageId);
    
    /// <summary>
    /// Starts execution without waiting - creates database record and returns immediately
    /// </summary>
    Task<(bool Success, int ExecutionRecordId, long? CatalogExecutionId, string? ErrorMessage)> StartPackageExecutionAsync(int packageId, string executedBy);
    
    /// <summary>
    /// Updates execution status by checking the catalog
    /// </summary>
    Task<bool> UpdateExecutionStatusAsync(int executionRecordId);
}
