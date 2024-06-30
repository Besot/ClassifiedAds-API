namespace AlutaMartAPI.Database;

public class UnitOfWork(AppDbContext context) : IDisposable, IUnitOfWork
{
    public AppDbContext Context { get; } = context;
    private bool _disposed = false;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Context.Dispose();
        }
        _disposed = true;
    }

    #endregion
}