namespace DataLifecycleManager.Domain.Common;

/// <summary>
/// Interface for entities that support audit tracking
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModifiedAt { get; set; }
    string? LastModifiedBy { get; set; }
    bool IsDeleted { get; set; }
}
