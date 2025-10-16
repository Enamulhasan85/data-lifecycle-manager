namespace DataLifecycleManager.Domain.Enums;

/// <summary>
/// Status of SSIS package
/// </summary>
public enum SSISPackageStatus
{
    Active = 1,
    Inactive = 2,
    InDevelopment = 3,
    Deprecated = 4
}
