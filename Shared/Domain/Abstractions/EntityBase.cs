using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shared.Domain.Abstractions;

public abstract class EntityBase
{
    /// <summary>
    /// Identificador único do documento.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    /// <summary>
    /// Data de criação do documento.
    /// </summary>
    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data da última atualização do documento.
    /// </summary>
    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica se o documento foi excluído logicamente.
    /// </summary>
    [BsonElement("isDeleted")]
    [BsonRepresentation(BsonType.Boolean)]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Exclui a entidade logicamente, marcando-a como excluída.
    /// </summary>
    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza os timestamps de atualização da entidade.
    /// </summary>
    public void UpdateTimestamps()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"Id: {Id}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}";
    }
}

public abstract class EntityBaseWithTenant : EntityBase
{
    /// <summary>
    /// Identificador do locatário (tenant).
    /// </summary>
    [BsonElement("tenantId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string TenantId { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, TenantId: {TenantId}";
    }
}
