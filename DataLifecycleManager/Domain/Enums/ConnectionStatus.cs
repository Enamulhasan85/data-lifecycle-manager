namespace DataLifecycleManager.Domain.Enums;

/// <summary>
/// Status of database connection
/// </summary>
public enum ConnectionStatus
{
    Active = 1,
    Inactive = 2,
    Testing = 3,
    Failed = 4
}
