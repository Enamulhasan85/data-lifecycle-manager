using System.ComponentModel.DataAnnotations;
using DataLifecycleManager.Domain.Common;
using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Domain.Entities;


public class SSISPackage : AuditableEntity<int>
{
    [Required]
    [MaxLength(200)]
    public string FolderName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ProjectName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string PackageName { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public SSISPackageStatus Status { get; set; } = SSISPackageStatus.Active;

    public int TimeoutMinutes { get; set; } = 60;

    /// <summary>
    /// Package parameters stored as JSON (key-value pairs)
    /// Example: {"SrcServer":"localhost","SrcDatabase":"DB1","DestServer":"localhost","DestDatabase":"DB2"}
    /// </summary>
    public string? PackageParameters { get; set; }

    public DateTime? LastExecutionDate { get; set; }

    public ExecutionStatus? LastExecutionStatus { get; set; }

    public virtual ICollection<SSISPackageExecution> Executions { get; set; } = new List<SSISPackageExecution>();
}
