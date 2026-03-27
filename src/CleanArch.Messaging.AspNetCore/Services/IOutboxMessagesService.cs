using CleanArch.Messaging.AspNetCore.Entities;

namespace CleanArch.Messaging.AspNetCore.Services;

public interface IOutboxMessagesService
{
    Task<IEnumerable<OutboxRecord>> GetUnprocessedListAsync(CancellationToken cancellationToken = default);

    Task<bool> IsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);
}