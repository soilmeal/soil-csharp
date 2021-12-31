using System;
using System.Text;

namespace Soil.Buffers;

public class InvalidBufferOperationException : InvalidOperationException
{
    public const string ReadIndexExceed = "ReadIndex is exceeded WrittenIndex.";

    public const string WrittenIndexExceed = "WrittenIndex is exceeded Capacity.";

    public const string InvalidEndianless = "Invalid endianless.";

    public const string MaxCapacityReached = "Max capacity reached.";

    public const string ReleaseTwice = "Release Twice.";

    public const string Uninitialized = "Uninitialized buffer.";

    private const string DefaultMessage = "Invalid buffer operation.";

    private readonly string _string;

    public InvalidBufferOperationException()
        : this(null)
    {
    }

    public InvalidBufferOperationException(string? message)
        : this(message, null)
    {
    }

    public InvalidBufferOperationException(string? message, Exception? innerException)
        : this(message, -1, -1, -1, innerException)
    {
    }

    public InvalidBufferOperationException(
        string? message,
        int readIndex,
        int writtenIndex,
        int length)
        : this(message, readIndex, writtenIndex, length, null)
    {
    }

    public InvalidBufferOperationException(
        string? message,
        int readIndex,
        int writtenIndex,
        int length,
        Exception? innerException)
        : base(message ?? DefaultMessage, innerException)
    {
        string actualMessage = base.Message;
        bool infoPassed = readIndex >= 0 && writtenIndex >= 0 && length > 0;

        int capacity = actualMessage.Length;
        capacity += infoPassed ? 30 : 0;

        StringBuilder builder = new StringBuilder(capacity);
        builder.Append(message);

        if (infoPassed)
        {
            builder.Append(" ReadIndex=")
                .Append(readIndex)
                .Append(", WrittenIndex=")
                .Append(writtenIndex)
                .Append(", Length=")
                .Append(length);
        }

        _string = builder.ToString();
    }

    public override string ToString()
    {
        return _string;
    }
}
