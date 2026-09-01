using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using MIN.Core.Cryptography.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Cryptography;

/// <summary>
/// Помошник с ключами
/// </summary>
public sealed class KeyProvider : IDisposable
{
    private const string ProtectorKey = "MIN.Core.Cryptography.KeyProtection";

    private readonly FileSystemKeyStorage storage;
    private readonly IDataProtector protector;
    private KeyPair? cachedKeys;
    private readonly SemaphoreSlim cacheLock = new(1, 1);
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="KeyProvider"/>
    /// </summary>
    public KeyProvider(IDataProtectionProvider dataProtection,
       IAppDataProvider appDataProvider,
       ILoggerProvider logger)
    {
        storage = new FileSystemKeyStorage(appDataProvider, logger);
        protector = dataProtection.CreateProtector(ProtectorKey);
    }

    /// <summary>
    /// Получить или сгенерировать локальную пару ключей
    /// </summary>
    public async Task<KeyPair> GetLocalKeysAsync()
    {
        if (cachedKeys != null)
        {
            return cachedKeys;
        }

        await cacheLock.WaitAsync();
        try
        {
            if (cachedKeys != null)
            {
                return cachedKeys;
            }

            cachedKeys = await storage.LoadLocalKeyPairAsync();

            if (cachedKeys == null)
            {
                cachedKeys = GenerateNewKeys();
                await storage.SaveLocalKeyPairAsync(cachedKeys);
            }

            return cachedKeys;
        }
        finally
        {
            cacheLock.Release();
        }
    }

    private async Task<ECDiffieHellman> GetEcdhPrivateKeyAsync()
    {
        var keys = await GetLocalKeysAsync();
        var decryptedPem = Unprotect(keys.EncryptedEcdhPrivateKeyPem);
        var ecdh = ECDiffieHellman.Create();
        ecdh.ImportFromPem(decryptedPem);

        return ecdh;
    }

    /// <summary>
    /// Вычислить общий секрет с собеседником по его публичному ECDH-ключу
    /// </summary>
    public async Task<byte[]> ComputeSharedSecretAsync(byte[] partnerPublicKeyBytes)
    {
        using var myEcdh = await GetEcdhPrivateKeyAsync();

        using var partnerEcdh = ECDiffieHellman.Create();
        partnerEcdh.ImportSubjectPublicKeyInfo(partnerPublicKeyBytes, out _);

        var sharedSecret = myEcdh.DeriveKeyFromHash(
            partnerEcdh.PublicKey,
            HashAlgorithmName.SHA256,
            null,
            null);

        var aesKey = HKDF.DeriveKey(
            ikm: sharedSecret,
            salt: null,
            info: "encryption"u8.ToArray(),
            outputLength: 32,
            hashAlgorithmName: HashAlgorithmName.SHA256);

        return aesKey;
    }

    // Loads stored partner key, derives shared secret. No file writes, no overwrite.
    // Returns null when: no stored key, or stored key corrupt (ImportSubjectPublicKeyInfo throws → catch + log warning).
    /// <summary>
    /// Загружает ключ партнёра и расчитывает секрет
    /// </summary>
    public async Task<byte[]?> TryComputeStoredSharedSecretAsync(Guid partnerId, CancellationToken ct = default)
    {
        var key = await GetPartnerPublicKeyAsync(partnerId);
        if (key == null)
        {
            return null;
        }

        return await ComputeSharedSecretAsync(key);
    }

    // SHA-256 over the raw SPKI bytes → 32-byte fingerprint. Static, pure, deterministic
    // (SPKI encoding is stable for the same key across restarts).
    public static byte[] ComputeKeyFingerprint(byte[] publicKeyBytes)
    {

    }

    /// <summary>
    /// Сохранить публичный ключ собеседника
    /// </summary>
    public async Task SavePartnerPublicKeyAsync(Guid partnerId, byte[] partnerPublicKeyBytes)
        => await storage.SavePartnerPublicKeyAsync(partnerId, partnerPublicKeyBytes);

    /// <summary>
    /// Получить сохранённый публичный ключ собеседника
    /// </summary>
    public async Task<byte[]?> GetPartnerPublicKeyAsync(Guid partnerId)
        => await storage.LoadPartnerPublicKeyAsync(partnerId);

    private KeyPair GenerateNewKeys()
    {
        using var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);

        return new KeyPair
        {
            EcdhPublicKeyPem = ecdh.ExportSubjectPublicKeyInfoPem(),
            EncryptedEcdhPrivateKeyPem = Protect(ecdh.ExportPkcs8PrivateKeyPem()),
            EcdhPublicKeyBytes = ecdh.ExportSubjectPublicKeyInfo(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private string Protect(string plainText)
    {
        var bytes = Encoding.UTF8.GetBytes(plainText);
        var protectedBytes = protector.Protect(bytes);
        return Convert.ToBase64String(protectedBytes);
    }

    private string Unprotect(string protectedBase64)
    {
        var protectedBytes = Convert.FromBase64String(protectedBase64);
        var bytes = protector.Unprotect(protectedBytes);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        cacheLock.Dispose();
        storage.Dispose();
        disposed = true;
    }
}
