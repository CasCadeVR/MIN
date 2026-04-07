using System.Security.Cryptography;
using MIN.Core.Cryptography.Contracts.Models;

namespace MIN.Core.Cryptography.Contracts.Interfaces;

/// <summary>
/// Помощник для ключей
/// </summary>
public interface IKeyProvider
{
    /// <summary>
    /// Получить или сгенерировать локальную пару ключей
    /// </summary>
    Task<KeyPair> GetLocalKeysAsync();

    /// <summary>
    /// Получить приватный ECDH-ключ для вычисления общего секрета
    /// </summary>
    Task<ECDiffieHellman> GetEcdhPrivateKeyAsync();

    /// <summary>
    /// Вычислить общий секрет с собеседником по его публичному ECDH-ключу
    /// </summary>
    Task<byte[]> ComputeSharedSecretAsync(byte[] partnerPublicKeyBytes);

    /// <summary>
    /// Сохранить публичный ключ собеседника (для TOFU)
    /// </summary>
    /// <param name="partnerId">Идентификатор партнёра (участника)</param>
    Task SavePartnerPublicKeyAsync(Guid partnerId, byte[] partnerPublicKeyBytes);

    /// <summary>
    /// Получить сохранённый публичный ключ собеседника
    /// </summary>
    /// <param name="partnerId">Идентификатор партнёра (участника)</param>
    Task<byte[]?> GetPartnerPublicKeyAsync(Guid partnerId);
}
