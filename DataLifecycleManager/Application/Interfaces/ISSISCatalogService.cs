using DataLifecycleManager.Application.DTOs.SSISPackage;

namespace DataLifecycleManager.Application.Interfaces;

public interface ISSISCatalogService
{
    Task<PackageExistenceResultDto> CheckPackageExistsAsync(
        string serverAddress,
        string catalogName,
        bool useWindowsAuth,
        string? username,
        string? password,
        string folderName,
        string projectName,
        string packageName);

    Task<CatalogExecutionResultDto> ExecutePackageAsync(
        string serverAddress,
        string catalogName,
        bool useWindowsAuth,
        string? username,
        string? password,
        string folderName,
        string projectName,
        string packageName,
        Dictionary<string, object>? parameters = null);
}
