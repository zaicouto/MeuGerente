using Modules.Users.Domain.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Core;

namespace Modules.Users.Domain.Entities;

public class User : BaseEntityWithTenant
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
    /// Set the password hash and update the last updated timestamp.
    /// </summary>
    public void SetPassword(string password, IPasswordHasher hasher)
    {
        PasswordHash = hasher.Hash(password);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verify if the provided password matches the stored password hash.
    /// </summary>
    public bool VerifyPassword(string password, IPasswordHasher hasher)
    {
        return hasher.Verify(password, PasswordHash);
    }
}
