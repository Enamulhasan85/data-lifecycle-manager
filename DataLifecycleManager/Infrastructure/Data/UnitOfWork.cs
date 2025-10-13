using System;
using System.Threading;
using System.Threading.Tasks;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataLifecycleManager.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private IDbContextTransaction? _currentTransaction;

        // Repository properties


        public UnitOfWork(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Apply audit fields before saving
            ApplyAuditFields();

            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Applies audit fields (CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy) 
        /// to all tracked AuditableEntity instances
        /// </summary>
        private void ApplyAuditFields()
        {
            var currentUserId = _currentUserService?.UserId;
            var currentTime = DateTime.UtcNow;

            foreach (var entry in _context.ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = currentTime;
                        entry.Entity.CreatedBy = currentUserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = currentTime;
                        entry.Entity.LastModifiedBy = currentUserId;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.LastModifiedAt = currentTime;
                        entry.Entity.LastModifiedBy = currentUserId;
                        break;
                }
            }
        }


        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await _currentTransaction?.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _currentTransaction?.RollbackAsync(cancellationToken)!;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
