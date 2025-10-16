using System.ComponentModel.DataAnnotations;
using DataLifecycleManager.Domain.Common;

namespace DataLifecycleManager.Domain.Entities;

/// <summary>
/// Junction table to associate database connections with SSIS packages (many-to-many)
/// </summary>
public class DatabaseConnectionPackage : AuditableEntity<int>
{
    public int DatabaseConnectionId { get; set; }

    public int SSISPackageId { get; set; }

    [MaxLength(100)]
    public string? ConnectionRole { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual DatabaseConnection DatabaseConnection { get; set; } = null!;
    public virtual SSISPackage SSISPackage { get; set; } = null!;
}
