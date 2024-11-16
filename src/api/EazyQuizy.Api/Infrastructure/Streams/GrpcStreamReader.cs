using EazyQuizy.Api.Protos.Files;
using Grpc.Core;

namespace EazyQuizy.Api.Infrastructure.Streams;

internal sealed class GrpcStreamReader(IAsyncStreamReader<FileChunk> reader) : Stream
{
	private ReadOnlyMemory<byte> _currentBuffer;
	private int _position = 0;
	public override void Flush()
	{
	}
	
	public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public override int Read(byte[] buffer, int offset, int count)
	{
		return ReadAsync(buffer, offset, count, new()).Result;
	}

	public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		if (_position >= _currentBuffer.Length)
		{
			if (!await reader.MoveNext(cancellationToken))
				return 0;
			var message = reader.Current;
			_currentBuffer = message.ChunkData.Memory;
			_position = 0;
		}
		var bytesToRead = Math.Min(count, _currentBuffer.Length - _position);
		_currentBuffer.Span.Slice(_position, bytesToRead).CopyTo(buffer.AsSpan(offset));
		_position += bytesToRead;
		return bytesToRead;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public override bool CanRead => true;
	public override bool CanSeek => false;
	public override bool CanWrite => false;
	public override long Length => throw new NotSupportedException("Length is not supported.");
	public override long Position
	{
		get => _position;
		set => throw new NotSupportedException("Setting Position is not supported.");
	}
}