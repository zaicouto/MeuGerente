namespace Shared.Domain.Exceptions;

// 404 Not Found
public class NotFoundException(string message) : Exception(message) { }

// 400 Bad Request
public class BadRequestException(string message) : Exception(message) { }

// 401 Unauthorized
public class UnauthorizedException() : Exception("Credenciais inválidas.") { }

// 403 Forbidden
public class ForbiddenException(string message) : Exception(message) { }

// 409 Conflict
public class ConflictException(string message) : Exception(message) { }

// 500 Internal Server Error
public class InternalServerErrorException(string message) : Exception(message) { }
