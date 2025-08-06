namespace Shared.Domain.Exceptions;

/// <summary>
/// Exceção para capturar erros de debug. Útil para identificar problemas específicos durante o desenvolvimento.
/// </summary>
/// <param name="message">Descrição da exceção.</param>
/// <param name="debugData">Dados da exceção.</param>
public class DebugException(string message, object? debugData = null) : Exception(message)
{
    public object? DebugData { get; } = debugData;

    public override string ToString()
    {
        string extra = DebugData != null ? $" : {DebugData}" : "";
        return $"[DebugException] {Message}{extra}";
    }
}
