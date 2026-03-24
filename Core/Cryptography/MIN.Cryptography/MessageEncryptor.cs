using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Stateless;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models.Enums;

namespace MIN.Cryptography
{
    /// <inheritdoc cref="IMessageEncryptor"/>
    public class MessageEncryptor(ILoggerProvider logger, IKeyProvider keyProvider) : IMessageEncryptor, IDisposable
    {
        private readonly ConcurrentDictionary<Guid, byte[]> sharedSecrets = new();
        private bool disposed;

        bool IMessageEncryptor.IsSessionInitialized(Guid partnerId)
            => sharedSecrets.ContainsKey(partnerId);

        async Task IMessageEncryptor.InitializeSessionWithPartnerAsync(Guid partnerId, byte[] partnerPublicKey)
        {
            var sharedSecret = await keyProvider.ComputeSharedSecretAsync(partnerPublicKey);
            sharedSecrets[partnerId] = sharedSecret;

            await keyProvider.SavePartnerPublicKeyAsync(partnerId, partnerPublicKey);
        }

        private byte[] GetSessionKey(Guid partnerId)
        {
            if (sharedSecrets.TryGetValue(partnerId, out var key))
            {
                return key;
            }

            throw new InvalidOperationException($"Session not initialized for partner: {partnerId}");
        }

        async Task<HandshakeMessage> IMessageEncryptor.CreateSelfHandshakeMessageAsync(ParticipantInfo selfParticipant)
        {
            var keys = await keyProvider.GetLocalKeysAsync();

            return new HandshakeMessage
            {
                Participant = selfParticipant,
                PublicKey = keys.EcdhPublicKeyBytes
            };
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

                var plaintextUtf8 = Encoding.UTF8.GetString(plaintext);
                var preview = plaintextUtf8.Length > 200
                    ? plaintextUtf8.Substring(0, 200) + "..."
                    : plaintextUtf8;

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

        bool IMessageEncryptor.IsEncrypted(byte[] data) => data.Length > 0 && (data[0] & 0x01) != 0;

        byte[] IMessageEncryptor.AddEncryptionHeader(byte[] encryptedData)
        {
            var header = new byte[] { 0x01 };
            return header.Concat(encryptedData).ToArray();
        }

        byte[] IMessageEncryptor.AddPlainHeader(byte[] plainData)
        {
            var header = new byte[] { 0x00 };
            return header.Concat(plainData).ToArray();
        }

        byte[] IMessageEncryptor.RemoveEncryptionHeader(byte[] data) => data.Skip(1).ToArray();

        void IDisposable.Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
        }
    }
}
