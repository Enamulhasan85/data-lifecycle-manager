using System.ComponentModel.DataAnnotations;
using DataLifecycleManager.Domain.Common;
using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Domain.Entities;

public class DatabaseConnection : AuditableEntity<int>
{
    [Required]
    [MaxLength(200)]
    public string ConnectionName { get; set; } = string.Empty;

    public DatabaseProvider Provider { get; set; }

    [Required]
    [MaxLength(500)]
    public string ServerAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DatabaseName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Username { get; set; }

    [MaxLength(500)]
    public string? EncryptedPassword { get; set; }

    public bool UseWindowsAuthentication { get; set; }

    public ConnectionStatus Status { get; set; } = ConnectionStatus.Active;
}
