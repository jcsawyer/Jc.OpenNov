namespace Jc.OpenNov.Utilities;

public sealed class SimpleRateLimiter
{
    private readonly int _limit;
    private readonly TimeSpan _interval;
    private readonly Queue<DateTime> _requests = new();
    private readonly object _lock = new();

    public SimpleRateLimiter(int limit, TimeSpan interval)
    {
        _limit = limit;
        _interval = interval;
    }

    public bool AllowRequest()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;

            while (_requests.Count > 0 && now - _requests.Peek() > _interval)
                _requests.Dequeue();

            if (_requests.Count < _limit)
            {
                _requests.Enqueue(now);
                return true;
            }

            return false;
        }
    }
    
    public bool RemoveRequest()
    {
        lock (_lock)
        {
            if (_requests.Count > 0)
            {
                _requests.Dequeue();
                return true;
            }
            return false;
        }
    }
}