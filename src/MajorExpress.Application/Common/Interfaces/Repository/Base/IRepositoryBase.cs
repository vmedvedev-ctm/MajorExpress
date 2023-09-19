using MajorExpress.Domain.Entities.Base;

namespace MajorExpress.Application.Common.Interfaces.Repository.Base;

public interface IRepositoryBase<TEntity>
    where TEntity : EntityBase
{
    Task AddRangeAsync(IEnumerable<TEntity> domainObjects, CancellationToken cancellationToken);

    Task RemoveRangeAsync(IEnumerable<TEntity> domainObjects, CancellationToken cancellationToken);

    ValueTask<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TEntity> UpdateAsync(TEntity domainObject, CancellationToken cancellationToken);

    Task<List<TEntity>> GetRangeByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<TEntity> AddAsync(TEntity domainObject, CancellationToken cancellationToken);

    Task<TEntity> GetOrAddAsync(TEntity domainObject, CancellationToken cancellationToken);

    Task RemoveAsync(TEntity domainObject, CancellationToken cancellationToken);

    Task<ICollection<TEntity>> GetNoTrackingListAsync(CancellationToken cancellationToken);
}
