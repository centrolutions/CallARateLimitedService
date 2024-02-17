using System.Collections.Concurrent;
using System.Diagnostics;

namespace RateLimitedClient;

class Throttler
{
    readonly int _callsPerSecond;
    readonly Stopwatch _stopwatch;
    readonly ConcurrentQueue<TimeSpan> _callQueue;
    readonly SemaphoreSlim _semaphore;

    int _itemsProcessed = 0;

    public Throttler(int callsPerSecond)
    {
        _callsPerSecond = callsPerSecond;
        _stopwatch = Stopwatch.StartNew();
        _callQueue = new ConcurrentQueue<TimeSpan>();
        _semaphore = new SemaphoreSlim(initialCount: _callsPerSecond, maxCount: _callsPerSecond);
    }

    public async Task Execute(string[] items, Func<string, Task> process)
    {
        var tasks = items.Select(async item =>
        {
            await _semaphore.WaitAsync();

            if (_itemsProcessed > _callsPerSecond || Interlocked.Increment(ref _itemsProcessed) > _callsPerSecond)
            {
                _callQueue.TryDequeue(out TimeSpan time);
                var timeSinceStart = _stopwatch.Elapsed - time;
                var remainingWaitTime = TimeSpan.FromSeconds(1) - timeSinceStart;
                if (remainingWaitTime > TimeSpan.Zero)
                {
                    await Task.Delay(remainingWaitTime);
                }
            }

            try
            {
                await process(item);
            }
            finally
            {
                _callQueue.Enqueue(_stopwatch.Elapsed);
                _semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }
}
