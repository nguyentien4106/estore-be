namespace Estore.Application.Dtos.Files;

public class ThrottledStream : Stream
{
    private readonly Stream _baseStream;
    private readonly int _bytesPerSecond;
    private readonly int _chunkSize = 1024;
    private readonly TimeSpan _delay;

    public ThrottledStream(Stream baseStream, int bytesPerSecond)
    {
        _baseStream = baseStream;
        _bytesPerSecond = bytesPerSecond;
        _delay = TimeSpan.FromSeconds((double)_chunkSize / _bytesPerSecond);
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        int remaining = count;
        int currentOffset = offset;

        while (remaining > 0)
        {
            int chunk = Math.Min(_chunkSize, remaining);
            await _baseStream.WriteAsync(buffer, currentOffset, chunk, cancellationToken);
            currentOffset += chunk;
            remaining -= chunk;

            if (remaining > 0)
            {
                await Task.Delay(_delay, cancellationToken);
            }
        }
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
    public override void Flush() => _baseStream.Flush();
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}