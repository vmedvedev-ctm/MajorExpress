using MajorExpress.Application.Common.Interfaces.Repository;
using MajorExpress.Domain.Entities;
using MajorExpress.Infrastructure.Persistence.Repository.Base;

using Microsoft.EntityFrameworkCore;

namespace MajorExpress.Infrastructure.Persistence.Repository;

public class CancelCommentRepository : RepositoryBase<CancelComment>, ICancelCommentRepository
{
    public CancelCommentRepository(IDbContextFactory<DatabaseContext> dbContextFactoryFactory)
        : base(dbContextFactoryFactory)
    {
    }
}
