using Shared.Domain.Exceptions;

namespace Modules.Orders.Domain.Exceptions;

public class OrderNotFoundException()
    : NotFoundException("Não foi possível encontrar o pedido.") { }

public class OrderBadRequestException(string message) : BadRequestException(message) { }
