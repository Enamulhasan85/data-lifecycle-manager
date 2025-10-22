using System.Text.Json;
using DataLifecycleManager.Application.DTOs.SSISPackage;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Application.Services;

public class SSISPackageService : CrudService<SSISPackage, int>, ISSISPackageService
{
    private readonly IRepository<SSISPackage, int> _repository;
    private readonly IRepository<SSISPackageExecution, int> _executionRepository;
    private readonly ISSISCatalogService _catalogService;
    private readonly IUnitOfWork _unitOfWork;

    public SSISPackageService(
        IRepository<SSISPackage, int> repository,
        IRepository<SSISPackageExecution, int> executionRepository,
        IUnitOfWork unitOfWork,
        ISSISCatalogService catalogService)
        : base(repository, unitOfWork)
    {
        _repository = repository;
        _executionRepository = executionRepository;
        _catalogService = catalogService;
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, PackageExistenceResultDto Result)> TestPackageConnectionAsync(int packageId)
    {
        var package = await _repository.GetByIdAsync(packageId);
        if (package == null)
        {
            return (false, new PackageExistenceResultDto
            {
                Success = false,
                ErrorMessage = "Package not found in database."
            });
        }

        var result = await _catalogService.CheckPackageExistsAsync(
            package.ServerAddress,
            package.CatalogName,
            package.UseWindowsAuthentication,
            package.Username,
            package.EncryptedPassword,
            package.FolderName,
            package.ProjectName,
            package.PackageName);

        return (true, result);
    }

    public async Task<(bool Success, CatalogExecutionResultDto Result)> ExecutePackageAsync(int packageId)
    {
        var package = await _repository.GetByIdAsync(packageId);
        if (package == null)
        {
            return (false, new CatalogExecutionResultDto
            {
                Success = false,
                ErrorMessage = "Package not found in database."
            });
        }

        Dictionary<string, object>? parameters = null;
        if (!string.IsNullOrWhiteSpace(package.PackageParameters))
        {
            try
            {
                parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(package.PackageParameters);
            }
            catch
            {
                return (false, new CatalogExecutionResultDto
                {
                    Success = false,
                    ErrorMessage = "Failed to parse package parameters."
                });
            }
        }

        var result = await _catalogService.ExecutePackageAsync(
            package.ServerAddress,
            package.CatalogName,
            package.UseWindowsAuthentication,
            package.Username,
            package.EncryptedPassword,
            package.FolderName,
            package.ProjectName,
            package.PackageName,
            parameters,
            package.TimeoutMinutes);

        return (true, result);
    }

    public async Task<(bool Success, int ExecutionRecordId, long? CatalogExecutionId, string? ErrorMessage)> StartPackageExecutionAsync(int packageId, string executedBy)
    {
        var package = await _repository.GetByIdAsync(packageId);
        if (package == null)
        {
            return (false, 0, null, "Package not found in database.");
        }

        Dictionary<string, object>? parameters = null;
        if (!string.IsNullOrWhiteSpace(package.PackageParameters))
        {
            try
            {
                parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(package.PackageParameters);
            }
            catch
            {
                return (false, 0, null, "Failed to parse package parameters.");
            }
        }

        // Start execution in SSIS catalog
        var result = await _catalogService.StartExecutionAsync(
            package.ServerAddress,
            package.CatalogName,
            package.UseWindowsAuthentication,
            package.Username,
            package.EncryptedPassword,
            package.FolderName,
            package.ProjectName,
            package.PackageName,
            parameters);

        if (!result.Success || result.ExecutionId == 0)
        {
            return (false, 0, null, result.ErrorMessage ?? "Failed to start execution");
        }

        // Create database record
        var execution = new SSISPackageExecution
        {
            SSISPackageId = packageId,
            CatalogExecutionId = result.ExecutionId,
            Status = ExecutionStatus.Running,
            StartTime = DateTime.UtcNow,
            ExecutedBy = executedBy,
            ExecutionParameters = package.PackageParameters,
            ExecutionLogs = result.Logs
        };

        await _executionRepository.AddAsync(execution);
        await _unitOfWork.SaveChangesAsync();

        return (true, execution.Id, result.ExecutionId, null);
    }

    public async Task<bool> UpdateExecutionStatusAsync(int executionRecordId)
    {
        var execution = await _executionRepository.GetByIdAsync(executionRecordId);
        if (execution == null || !execution.CatalogExecutionId.HasValue)
        {
            return false;
        }

        var package = await _repository.GetByIdAsync(execution.SSISPackageId);
        if (package == null)
        {
            return false;
        }

        // Get current status from catalog
        var statusResult = await _catalogService.GetExecutionStatusAsync(
            package.ServerAddress,
            package.CatalogName,
            package.UseWindowsAuthentication,
            package.Username,
            package.EncryptedPassword,
            execution.CatalogExecutionId.Value);

        if (!string.IsNullOrWhiteSpace(statusResult.Status))
        {
            // Update execution record
            execution.Status = statusResult.Status switch
            {
                "Succeeded" => ExecutionStatus.Succeeded,
                "Failed" => ExecutionStatus.Failed,
                "Cancelled" => ExecutionStatus.Cancelled,
                "Running" => ExecutionStatus.Running,
                "Pending" => ExecutionStatus.Pending,
                "Created" => ExecutionStatus.Pending,
                "Timeout" => ExecutionStatus.Timeout,
                _ => ExecutionStatus.Failed
            };

            // Update logs if available
            if (!string.IsNullOrWhiteSpace(statusResult.Logs))
            {
                execution.ExecutionLogs = statusResult.Logs;
            }

            // If execution is completed, set end time and calculate duration
            if (execution.Status == ExecutionStatus.Succeeded ||
                execution.Status == ExecutionStatus.Failed ||
                execution.Status == ExecutionStatus.Cancelled)
            {
                if (!execution.EndTime.HasValue)
                {
                    execution.EndTime = DateTime.UtcNow;
                    if (execution.StartTime.HasValue)
                    {
                        execution.DurationSeconds = (int)(execution.EndTime.Value - execution.StartTime.Value).TotalSeconds;
                    }
                }
            }

            // Update error message if failed
            if (execution.Status == ExecutionStatus.Failed && !string.IsNullOrWhiteSpace(statusResult.ErrorMessage))
            {
                execution.ErrorMessage = statusResult.ErrorMessage;
            }

            await _executionRepository.UpdateAsync(execution);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        return false;
    }
}
