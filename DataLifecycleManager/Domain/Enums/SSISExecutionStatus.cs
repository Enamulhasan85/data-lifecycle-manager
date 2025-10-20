namespace DataLifecycleManager.Domain.Enums;

public enum SSISExecutionStatus
{
    Created = 1,
    Running = 2,
    Canceled = 3,
    Failed = 4,
    Pending = 5,
    EndedUnexpectedly = 6,
    Succeeded = 7,
    Stopping = 8,
    Completed = 9
}
