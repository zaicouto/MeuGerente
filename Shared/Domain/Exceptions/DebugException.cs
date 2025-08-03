namespace Shared.Domain.Exceptions;

public class DebugException(string message, object? debugData = null) : Exception(message)
{
    public object? DebugData { get; } = debugData;

    public override string ToString()
    {
        string extra = DebugData != null ? $" : {DebugData}" : "";
        return $"[DebugException] {Message}{extra}";
    }
}
