using DataLifecycleManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLifecycleManager.Infrastructure.Configuration;

/// <summary>
/// Entity Framework configuration for SSISPackageExecution
/// </summary>
public class SSISPackageExecutionConfiguration : IEntityTypeConfiguration<SSISPackageExecution>
{
    public void Configure(EntityTypeBuilder<SSISPackageExecution> builder)
    {
        // Table name
        builder.ToTable("SSISPackageExecutions");

        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for performance optimization
        // Index on CreatedAt for pagination queries (most important - used in OrderBy)
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_SSISPackageExecutions_CreatedAt")
            .IsDescending(); // Optimize for DESC ordering

        // Index on SSISPackageId (foreign key)
        builder.HasIndex(e => e.SSISPackageId)
            .HasDatabaseName("IX_SSISPackageExecutions_SSISPackageId");

        // Composite index for common query patterns (Status + CreatedAt)
        builder.HasIndex(e => new { e.Status, e.CreatedAt })
            .HasDatabaseName("IX_SSISPackageExecutions_Status_CreatedAt")
            .IsDescending(false, true); // Status ASC, CreatedAt DESC

        // Index on CatalogExecutionId for lookups
        builder.HasIndex(e => e.CatalogExecutionId)
            .HasDatabaseName("IX_SSISPackageExecutions_CatalogExecutionId");

        // Index on ExecutedBy for filtering by user
        builder.HasIndex(e => e.ExecutedBy)
            .HasDatabaseName("IX_SSISPackageExecutions_ExecutedBy");

        // Relationship configuration
        builder.HasOne(e => e.SSISPackage)
            .WithMany()
            .HasForeignKey(e => e.SSISPackageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
