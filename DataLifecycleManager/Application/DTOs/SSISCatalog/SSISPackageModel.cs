namespace DataLifecycleManager.Application.DTOs.SSISCatalog;

public class SSISPackageModel
{
    public long PackageId { get; set; }
    public long ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
}
