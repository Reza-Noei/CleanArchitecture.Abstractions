using CleanArch.Messaging.AspNetCore.Entities;

namespace CleanArch.Messaging.AspNetCore.Queues;

public interface IInboxMessageQueue
{
    void Enqueue(InboxRecord message);

    Task<InboxRecord?> DequeueAsync(CancellationToken cancellationToken = default);
}