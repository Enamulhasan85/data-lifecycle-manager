namespace DataLifecycleManager.Domain.Enums;

/// <summary>
/// Status of SSIS package execution
/// </summary>
public enum ExecutionStatus
{
    Pending = 1,
    Running = 2,
    Succeeded = 3,
    Failed = 4,
    Cancelled = 5,
    Timeout = 6
}
