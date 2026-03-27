using CleanArch.Messaging.AspNetCore.Entities;
using System.Collections.Concurrent;

namespace CleanArch.Messaging.AspNetCore.Queues;

internal sealed class InboxMessageQueue : IInboxMessageQueue
{
    private readonly SemaphoreSlim _signal = new(0);
    private readonly ConcurrentQueue<InboxRecord> _queue = new();

    public void Enqueue(InboxRecord message)
    {
        ArgumentNullException.ThrowIfNull(message);

        _queue.Enqueue(message);
        _signal.Release();
    }

    public async Task<InboxRecord?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        await _signal.WaitAsync(cancellationToken);

        _queue.TryDequeue(out var message);
        return message;
    }
}