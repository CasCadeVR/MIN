using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Contracts.Models.Messages;

namespace MIN.Services.Connection.Cryptographing
{
    /// <inheritdoc cref="ICryptoProvider"/>
    public class CryptoProvider(IKeyProvider keyProvider) : ICryptoProvider, IDisposable
    {
        private readonly ConcurrentDictionary<Guid, byte[]> sharedSecrets = new();
        private bool disposed;

        bool ICryptoProvider.IsSessionInitialized(Guid partnerId)
            => sharedSecrets.ContainsKey(partnerId);

        async Task ICryptoProvider.InitializeSessionAsync(Guid partnerId, HandshakeMessage handshake)
        {
            var sharedSecret = await keyProvider.ComputeSharedSecretAsync(handshake.EcdhPublicKeyPem);
            sharedSecrets[partnerId] = sharedSecret;

            // Сохраняем публичный ключ партнёра для будущих проверок (TOFU)
            await keyProvider.SavePartnerPublicKeyAsync(partnerId, handshake.EcdhPublicKeyPem);
        }

        private byte[] GetSessionKey(Guid partnerId)
        {
            if (sharedSecrets.TryGetValue(partnerId, out var key))
                return key;
            throw new InvalidOperationException($"Session not initialized for partner: {partnerId}");
        }

        async Task<HandshakeMessage> ICryptoProvider.CreateHandshakeAsync(Guid id)
        {
            var keys = await keyProvider.GetLocalKeysAsync();

            return new HandshakeMessage
            {
                UserId = id,
                EcdhPublicKeyPem = keys.EcdhPublicKeyPem,
                RsaPublicKeyPem = keys.RsaPublicKeyPem,
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        byte[] ICryptoProvider.EncryptMessage(byte[] plaintext, Guid partnerId)
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

        byte[] ICryptoProvider.DecryptMessage(byte[] encryptedData, Guid partnerId)
        {
            var key = GetSessionKey(partnerId);

            if (encryptedData.Length < 12 + 16)
                throw new InvalidDataException("Invalid encrypted data");

            var iv = encryptedData.AsSpan(0, 12).ToArray();
            var authTag = encryptedData.AsSpan(encryptedData.Length - 16, 16).ToArray();
            var ciphertext = encryptedData.AsSpan(12, encryptedData.Length - 28).ToArray();

            using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);
            var plaintext = new byte[ciphertext.Length];
            aesGcm.Decrypt(iv, ciphertext, authTag, plaintext);

            return plaintext;
        }

        void IDisposable.Dispose()
        {
            if (disposed) return;
            disposed = true;
        }
    }
}
