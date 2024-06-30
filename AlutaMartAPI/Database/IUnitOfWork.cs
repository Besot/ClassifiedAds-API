namespace AlutaMartAPI.Database;

public interface IUnitOfWork : IDisposable
{
    AppDbContext Context { get; }
    Task CommitAsync(CancellationToken cancellationToken = default);
}