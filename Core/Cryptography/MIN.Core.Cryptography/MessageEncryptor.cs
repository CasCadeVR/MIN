using System.Collections.Concurrent;
using System.Security.Cryptography;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;
using MIN.Core.Cryptography.Contracts.Interfaces;

namespace MIN.Core.Cryptography;

/// <inheritdoc cref="IMessageEncryptor"/>
public class MessageEncryptor : IMessageEncryptor, IDisposable
{
    private readonly ILoggerProvider logger;
    private readonly IKeyProvider keyProvider;
    private readonly ConcurrentDictionary<Guid, byte[]> sharedSecrets = new();
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MessageEncryptor"/>
    /// </summary>
    public MessageEncryptor(ILoggerProvider logger, IKeyProvider keyProvider)
    {
        this.logger = logger;
        this.keyProvider = keyProvider;
    }

    bool IMessageEncryptor.IsSessionInitialized(Guid partnerId)
        => sharedSecrets.ContainsKey(partnerId);

    async Task IMessageEncryptor.InitializeSessionWithPartnerAsync(Guid partnerId, byte[] partnerPublicKey)
    {
        var sharedSecret = await keyProvider.ComputeSharedSecretAsync(partnerPublicKey);
        sharedSecrets[partnerId] = sharedSecret;

        await keyProvider.SavePartnerPublicKeyAsync(partnerId, partnerPublicKey);
    }

    async Task<byte[]> IMessageEncryptor.GetLocalPublicKey()
    {
        var keys = await keyProvider.GetLocalKeysAsync();
        return keys.EcdhPublicKeyBytes;
    }

    byte[] IMessageEncryptor.EncryptMessage(byte[] plaintext, Guid partnerId)
    {
        var key = GetSessionKey(partnerId);
        var iv = RandomNumberGenerator.GetBytes(12);

        using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        var ciphertext = new byte[plaintext.Length];
        var authTag = new byte[16];
        aesGcm.Encrypt(iv, plaintext, ciphertext, authTag);

        using var ms = new MemoryStream();
        ms.Write(iv, 0, 12);
        ms.Write(ciphertext, 0, ciphertext.Length);
        ms.Write(authTag, 0, 16);

        return ms.ToArray();
    }

    byte[] IMessageEncryptor.DecryptMessage(byte[] encryptedData, Guid partnerId)
    {
        var key = GetSessionKey(partnerId);

        if (encryptedData.Length < 12 + 16)
        {
            throw new InvalidDataException($"Invalid encrypted data: length {encryptedData.Length} < 28");
        }

        var iv = encryptedData.AsSpan(0, 12).ToArray();
        var authTag = encryptedData.AsSpan(encryptedData.Length - 16, 16).ToArray();
        var ciphertextLength = encryptedData.Length - 12 - 16;
        var ciphertext = encryptedData.AsSpan(12, ciphertextLength).ToArray();

        using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        var plaintext = new byte[ciphertext.Length];

        try
        {
            aesGcm.Decrypt(iv, ciphertext, authTag, plaintext);
            return plaintext;
        }
        catch (CryptographicException ex) when (ex is AuthenticationTagMismatchException)
        {
            logger.Log($"Ошибка аутентификации — ключи не совпадают", LogLevel.Error);
            throw;
        }
        catch (CryptographicException ex)
        {
            logger.Log($"Возникла крипто-ошибка: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    private byte[] GetSessionKey(Guid partnerId)
    {
        if (sharedSecrets.TryGetValue(partnerId, out var key))
        {
            return key;
        }

        throw new InvalidOperationException($"Сессия не инициализирована для партнёра с id: {partnerId}");
    }

    void IDisposable.Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
    }
}
