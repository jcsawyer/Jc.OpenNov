namespace Jc.OpenNov;

public sealed class TransceiveResult : IDisposable, IAsyncDisposable
{
    public MemoryStream? Content { get; init; }
    public bool Success { get; init; }

    public void Dispose()
    {
        Content?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (Content is not null)
        {
            await Content.DisposeAsync();
        }
    }
}