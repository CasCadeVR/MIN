namespace MIN.Core.Cryptography.Contracts.Models;

/// <summary>
/// Пара ключей
/// </summary>
public class KeyPair
{
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Публичный ключ для шифрования (ECDH)
    /// </summary>
    public string EcdhPublicKeyPem { get; init; } = string.Empty;

    /// <summary>
    /// Зашифрованный приватный ключ для шифрования (ECDH)
    /// </summary>
    public string EncryptedEcdhPrivateKeyPem { get; init; } = string.Empty;

    /// <summary>
    /// Для сетевого обмена
    /// </summary>
    public byte[] EcdhPublicKeyBytes { get; init; } = null!;
}
