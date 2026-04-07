namespace MIN.Core.Cryptography.Contracts.Interfaces;

/// <summary>
/// Помощник в шифровании
/// </summary>
public interface IMessageEncryptor
{
    /// <summary>
    /// Проверить, инициализирована ли сессия с собеседником
    /// </summary>
    bool IsSessionInitialized(Guid partnerId);

    /// <summary>
    /// Вызывается при первом контакте с собеседником
    /// </summary>
    /// <param name="partnerId">Идентификатор партнёра (участника)</param>
    Task InitializeSessionWithPartnerAsync(Guid partnerId, byte[] partnerPublicKey);

    /// <summary>
    /// Создать сообщения рукопожатия
    /// </summary>
    Task<byte[]> GetLocalPublicKey();

    /// <summary>
    /// Закодировать сообщение
    /// </summary>
    /// <param name="partnerId">Идентификатор партнёра (участника)</param>
    byte[] EncryptMessage(byte[] data, Guid partnerId);

    /// <summary>
    /// Раскодировать сообщение
    /// </summary>
    /// <param name="partnerId">Идентификатор партнёра (участника)</param>
    byte[] DecryptMessage(byte[] encryptedData, Guid partnerId);

    /// <summary>
    /// Добавить заголовок шифрования
    /// </summary>
    byte[] AddEncryptionHeader(byte[] secretData);

    /// <summary>
    /// Добавить пустой заголовок
    /// </summary>
    byte[] AddPlainHeader(byte[] plainData);

    /// <summary>
    /// Проверяет по заголовку, зашифровано ли сообщение
    /// </summary>
    bool IsEncrypted(byte[] data);

    /// <summary>
    /// Убрать заголовок шифрования
    /// </summary>
    byte[] RemoveEncryptionHeader(byte[] data);
}
