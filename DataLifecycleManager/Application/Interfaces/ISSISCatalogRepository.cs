using DataLifecycleManager.Application.DTOs.SSISCatalog;

namespace DataLifecycleManager.Application.Interfaces;

public interface ISSISCatalogRepository
{
    Task<SSISFolderModel?> GetFolderAsync(string connectionString, string catalogName, string folderName);
    Task<SSISProjectModel?> GetProjectAsync(string connectionString, string catalogName, long folderId, string projectName);
    Task<SSISPackageModel?> GetPackageAsync(string connectionString, string catalogName, long projectId, string packageName);
    Task<List<SSISParameterModel>> GetProjectParametersAsync(string connectionString, string catalogName, long projectId);
    Task<long> CreateExecutionAsync(string connectionString, string catalogName, long projectId, string packageName);
    Task SetExecutionParameterAsync(string connectionString, long executionId, string parameterName, object value);
    Task StartExecutionAsync(string connectionString, long executionId);
    Task<SSISExecutionModel?> GetExecutionStatusAsync(string connectionString, long executionId);
    Task<List<string>> GetExecutionMessagesAsync(string connectionString, long executionId);
}
