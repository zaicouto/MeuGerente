﻿namespace Shared.Infrastructure.Base;

public class ApiResponse(bool success, string message, object? data)
{
    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
    public object? Data { get; set; } = data;
}
