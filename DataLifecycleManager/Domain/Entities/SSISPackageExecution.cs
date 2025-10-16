using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLifecycleManager.Domain.Common;
using DataLifecycleManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataLifecycleManager.Domain.Entities;

/// <summary>
/// Represents an execution instance of an SSIS package
/// </summary>
public class SSISPackageExecution : AuditableEntity<int>
{
    public int SSISPackageId { get; set; }

    public long? CatalogExecutionId { get; set; }

    public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? DurationSeconds { get; set; }

    [MaxLength(200)]
    public string? ExecutedBy { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? ExecutionParameters { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? ExecutionLogs { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? ErrorMessage { get; set; }

    public virtual SSISPackage SSISPackage { get; set; } = null!;
}
