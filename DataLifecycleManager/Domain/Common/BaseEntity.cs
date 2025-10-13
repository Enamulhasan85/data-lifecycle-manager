using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLifecycleManager.Domain.Common;

public abstract class BaseEntity<TKey>
{
    public TKey Id { get; protected set; } = default!;
}

public abstract class AuditableEntity<TKey> : BaseEntity<TKey>, IAuditable
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(450)]
    public string? CreatedBy { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    [MaxLength(450)]
    public string? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

    public void MarkDeleted(string? userId = null)
    {
        IsDeleted = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = userId;
    }
}

