using CleanArch.Messaging.AspNetCore.Entities;

namespace CleanArch.Messaging.AspNetCore.Queues;

public interface IOutboxMessageQueue
{
    void Enqueue(OutboxRecord message);

    Task<OutboxRecord?> DequeueAsync(CancellationToken cancellationToken = default);
}