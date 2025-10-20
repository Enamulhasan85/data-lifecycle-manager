using DataLifecycleManager.Application.DTOs.SSISPackage;
using DataLifecycleManager.Domain.Entities;

namespace DataLifecycleManager.Application.Interfaces;

public interface ISSISPackageService : ICrudService<SSISPackage, int>
{
    Task<(bool Success, PackageExistenceResultDto Result)> TestPackageConnectionAsync(int packageId);
    Task<(bool Success, CatalogExecutionResultDto Result)> ExecutePackageAsync(int packageId);
}
