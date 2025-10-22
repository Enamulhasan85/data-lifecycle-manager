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
        Dictionary<string, object>? parameters,
        int timeoutMinutes);

    /// <summary>
    /// Starts execution without waiting for completion - returns execution ID immediately
    /// </summary>
    Task<CatalogExecutionResultDto> StartExecutionAsync(
        string serverAddress,
        string catalogName,
        bool useWindowsAuth,
        string? username,
        string? password,
        string folderName,
        string projectName,
        string packageName,
        Dictionary<string, object>? parameters);

    /// <summary>
    /// Gets the current status of an execution by execution ID
    /// </summary>
    Task<CatalogExecutionResultDto> GetExecutionStatusAsync(
        string serverAddress,
        string catalogName,
        bool useWindowsAuth,
        string? username,
        string? password,
        long executionId);
}
