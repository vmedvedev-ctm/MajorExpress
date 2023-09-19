using MajorExpress.Application.Common.Interfaces.Repository.Base;
using MajorExpress.Domain.Entities.Base;

using Microsoft.EntityFrameworkCore;

namespace MajorExpress.Infrastructure.Persistence.Repository.Base;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
    where TEntity : EntityBase
{
    protected readonly IDbContextFactory<DatabaseContext> DbContextFactory;

    protected RepositoryBase(IDbContextFactory<DatabaseContext> dbContextFactoryFactory) => DbContextFactory = dbContextFactoryFactory;

    public async Task AddRangeAsync(IEnumerable<TEntity> domainObjects, CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.Set<TEntity>().AddRangeAsync(domainObjects, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> domainObjects, CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);
        dbContext.Set<TEntity>().RemoveRange(domainObjects);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity domainObject, CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        var item = dbContext.Set<TEntity>().Update(domainObject);
        await dbContext.SaveChangesAsync(cancellationToken);
        return item.Entity;
    }

    public async Task<List<TEntity>> GetRangeByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Set<TEntity>().Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity domainObject, CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        var item = await dbContext.Set<TEntity>().AddAsync(domainObject, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return item.Entity;
    }

    public async Task<TEntity> GetOrAddAsync(TEntity domainObject, CancellationToken cancellationToken)
    {
        if (domainObject.Id.Equals(default)) return await AddAsync(domainObject, cancellationToken);

        var item = await GetByIdAsync(domainObject.Id, cancellationToken);

        if (item == null) return await AddAsync(domainObject, cancellationToken);

        return item;
    }

    public async Task RemoveAsync(TEntity domainObject, CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        dbContext.Remove(domainObject);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ICollection<TEntity>> GetNoTrackingListAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
    }
}
