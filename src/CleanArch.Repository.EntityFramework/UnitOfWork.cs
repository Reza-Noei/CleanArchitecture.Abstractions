using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Repository.EntityFramework;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }


    private readonly DbContext _dbContext;
}