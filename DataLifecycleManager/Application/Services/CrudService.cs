using System.Linq.Expressions;
using DataLifecycleManager.Application.DTOs.Common;
using DataLifecycleManager.Application.Interfaces;

namespace DataLifecycleManager.Application.Services;

public class CrudService<TEntity, TKey> : ICrudService<TEntity, TKey>
    where TEntity : class
{
    private readonly IRepository<TEntity, TKey> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CrudService(IRepository<TEntity, TKey> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    #region Basic CRUD Operations

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity?> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return null;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return false;

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Query Operations

    public async Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await _repository.FindAsync(predicate, cancellationToken, includes);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await _repository.FirstOrDefaultAsync(predicate, cancellationToken, includes);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return await _repository.CountAsync(predicate, cancellationToken);
    }

    #endregion

    #region Pagination

    public async Task<PaginatedResult<TResult>> GetPaginatedAsync<TResult>(
        int page,
        int pageSize,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        if (page <= 0)
            throw new ArgumentException("Page must be greater than 0", nameof(page));
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _repository.GetPaginatedAsync(
            page,
            pageSize,
            selector,
            predicate,
            orderBy,
            orderByDescending,
            cancellationToken,
            includes);
    }

    #endregion

}

