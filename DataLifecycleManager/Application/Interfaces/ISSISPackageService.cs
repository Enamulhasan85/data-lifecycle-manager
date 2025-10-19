using DataLifecycleManager.Domain.Entities;

namespace DataLifecycleManager.Application.Interfaces;

public interface ISSISPackageService : ICrudService<SSISPackage, int>
{
    Task<bool> PackageExistsAsync(string folderName, string projectName, string packageName, int? excludeId = null);
}
