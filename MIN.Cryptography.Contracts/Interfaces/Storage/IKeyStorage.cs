using MIN.Cryptography.Contracts.Models;

namespace MIN.Cryptography.Contracts.Interfaces.Storage;

/// <summary>
/// Предоставляет методы для сохранения и загрузки криптографических ключей
/// </summary>
public interface IKeyStorage
{
    /// <summary>
    /// Загружает локальную пару ключей из хранилища
    /// </summary>
    Task<KeyPair?> LoadLocalKeyPairAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет локальную пару ключей в хранилище
    /// </summary>
    Task SaveLocalKeyPairAsync(KeyPair keyPair, CancellationToken cancellationToken = default);

    /// <summary>
    /// Загружает словарь публичных ключей партнёров
    /// </summary>
    Task<Dictionary<Guid, byte[]>> LoadPartnerPublicKeysAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет публичный ключ партнёра в хранилище
    /// </summary>
    Task SavePartnerPublicKeyAsync(Guid partnerId, byte[] publicKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Загружает публичный ключ партнёра по его идентификатору
    /// </summary>
    Task<byte[]?> LoadPartnerPublicKeyAsync(Guid partnerId, CancellationToken cancellationToken = default);
}
