using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shared.Core;

public abstract class BaseEntity
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
}
