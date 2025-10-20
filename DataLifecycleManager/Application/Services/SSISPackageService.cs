using System.Text.Json;
using DataLifecycleManager.Application.DTOs.SSISPackage;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Entities;

namespace DataLifecycleManager.Application.Services;

public class SSISPackageService : CrudService<SSISPackage, int>, ISSISPackageService
{
    private readonly IRepository<SSISPackage, int> _repository;
    private readonly ISSISCatalogService _catalogService;

    public SSISPackageService(
        IRepository<SSISPackage, int> repository,
        IUnitOfWork unitOfWork,
        ISSISCatalogService catalogService)
        : base(repository, unitOfWork)
    {
        _repository = repository;
        _catalogService = catalogService;
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
            parameters);

        return (true, result);
    }
}
