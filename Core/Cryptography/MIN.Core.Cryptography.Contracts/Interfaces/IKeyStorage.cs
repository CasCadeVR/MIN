using MIN.Core.Cryptography.Contracts.Models;

namespace MIN.Core.Cryptography.Contracts.Interfaces;

/// <summary>
/// Помощник сохранения и загрузки криптографических ключей
/// </summary>
public interface IKeyStorage
{
    /// <summary>
    /// Загрузить локальную пару ключей из хранилища
    /// </summary>
    Task<KeyPair?> LoadLocalKeyPairAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить локальную пару ключей в хранилище
    /// </summary>
    Task SaveLocalKeyPairAsync(KeyPair keyPair, CancellationToken cancellationToken = default);

    /// <summary>
    /// Загрузить словарь публичных ключей партнёров
    /// </summary>
    Task<Dictionary<Guid, byte[]>> LoadPartnerPublicKeysAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить публичный ключ партнёра в хранилище
    /// </summary>
    /// <param name="partnerId">Идентификатор партнёра (участника)</param>
    Task SavePartnerPublicKeyAsync(Guid partnerId, byte[] publicKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Загрузить публичный ключ партнёра по его идентификатору
    /// </summary>
    /// <param name="partnerId">Идентификатор партнёра (участника)</param>
    Task<byte[]?> LoadPartnerPublicKeyAsync(Guid partnerId, CancellationToken cancellationToken = default);
}
