using System.Security.Cryptography;
using System.Text;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Cryptography.Contracts.Models;

namespace MIN.Core.Cryptography;

/// <inheritdoc cref="IKeyProvider"/>
public sealed class KeyProvider : IKeyProvider, IDisposable
{
    private readonly IKeyStorage storage;
    private KeyPair? cachedKeys;
    private readonly SemaphoreSlim cacheLock = new(1, 1);
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="KeyProvider"/>
    /// </summary>
    public KeyProvider(IKeyStorage storage)
    {
        this.storage = storage;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<ECDiffieHellman> GetEcdhPrivateKeyAsync()
    {
        var keys = await GetLocalKeysAsync();
        var decryptedPem = Unprotect(keys.EncryptedEcdhPrivateKeyPem);
        var ecdh = ECDiffieHellman.Create();
        ecdh.ImportFromPem(decryptedPem);

        return ecdh;
    }

    async Task<byte[]> IKeyProvider.ComputeSharedSecretAsync(byte[] partnerPublicKeyBytes)
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

    async Task IKeyProvider.SavePartnerPublicKeyAsync(Guid partnerId, byte[] partnerPublicKeyBytes)
    {
        await storage.SavePartnerPublicKeyAsync(partnerId, partnerPublicKeyBytes);
    }

    async Task<byte[]?> IKeyProvider.GetPartnerPublicKeyAsync(Guid partnerId)
    {
        return await storage.LoadPartnerPublicKeyAsync(partnerId);
    }

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

    private static string Protect(string plainText)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("К сожалению, пока только на Windows");
        }

        var bytes = Encoding.UTF8.GetBytes(plainText);
        var protectedBytes = ProtectedData.Protect(
            bytes,
            null,
            DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedBytes);
    }

    private static string Unprotect(string protectedBase64)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("К сожалению, пока только на Windows");
        }

        var protectedBytes = Convert.FromBase64String(protectedBase64);
        var bytes = ProtectedData.Unprotect(
            protectedBytes,
            null,
            DataProtectionScope.CurrentUser);

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
        disposed = true;
    }
}
