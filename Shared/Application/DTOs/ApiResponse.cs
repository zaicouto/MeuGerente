namespace Shared.Application.DTOs;

public record ApiResponse(bool Success, string Message, object? Data);
