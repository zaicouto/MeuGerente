using Modules.Orders.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Core;

namespace Modules.Orders.Domain.Entities;

public class Order : BaseEntityWithTenant
{
    /// <summary>
    /// Status atual do pedido.
    /// </summary>
    [BsonElement("status")]
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Lista de itens do pedido.
    /// </summary>
    [BsonElement("items")]
    public ICollection<OrderItem> Items { get; private set; } = [];

    /// <summary>
    /// ID do cliente que fez o pedido.
    /// </summary>
    [BsonElement("clientId")]
    public required string ClientId { get; set; }

    /// <summary>
    /// Atualiza o status do pedido, validando transições.
    /// </summary>
    /// <param name="newStatus">Novo status.</param>
    /// <exception cref="InvalidOperationException">Lança se a transição não for permitida.</exception>
    public void UpdateStatus(OrderStatus newStatus, bool isAdmin = false)
    {
        if (!isAdmin)
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException(
                    "Não é possível alterar o status de um pedido já entregue."
                );

            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException(
                    "Não é possível alterar o status de um pedido cancelado."
                );

            if (Status == OrderStatus.Confirmed && newStatus == OrderStatus.Pending)
                throw new InvalidOperationException(
                    "Não é possível voltar de Confirmado para Pendente."
                );
        }

        Status = newStatus;
    }

    /// <summary>
    /// Substitui todos os itens do pedido por uma nova lista.
    /// </summary>
    /// <param name="newItems">Nova lista de itens.</param>
    public void UpdateItems(IEnumerable<OrderItem> newItems)
    {
        ArgumentNullException.ThrowIfNull(newItems, nameof(newItems));
        Items = [.. newItems];
    }
}
