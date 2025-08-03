using Modules.Users.Domain.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Abstractions;

namespace Modules.Users.Domain.Entities;

public class User : EntityBaseWithTenant
{
    [BsonElement("email")]
    public required string Email { get; set; }

    [BsonElement("password")]
    private string PasswordHash { get; set; } = string.Empty;

    [BsonElement("firstName")]
    public required string FirstName { get; set; }

    [BsonElement("lastName")]
    public required string LastName { get; set; }

    [BsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Define o hash da senha e atualiza o timestamp de atualização.
    /// </summary>
    public void SetPassword(string password, IPasswordHasher hasher)
    {
        PasswordHash = hasher.Hash(password);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica se a senha fornecida corresponde ao hash da senha armazenado.
    /// </summary>
    public bool VerifyPassword(string password, IPasswordHasher hasher)
    {
        return hasher.Verify(password, PasswordHash);
    }
}
