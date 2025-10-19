using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Entities;

namespace DataLifecycleManager.Application.Services;

public class SSISPackageService : CrudService<SSISPackage, int>, ISSISPackageService
{
    private readonly IRepository<SSISPackage, int> _repository;

    public SSISPackageService(
        IRepository<SSISPackage, int> repository,
        IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
        _repository = repository;
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
}
