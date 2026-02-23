using System.Security.Cryptography;
using System.Text;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;

namespace MIN.Services.Connection.Cryptographing
{
    /// <inheritdoc cref="ICryptoProvider"/>
    public class CryptoProvider(IKeyProvider keyProvider) : ICryptoProvider, IDisposable
    {
        private readonly byte[] aesKey = keyProvider.GetOrCreateAesKey();
        private bool disposed;

        /// <inheritdoc cref="ICryptoProvider.RoomName"/>
        public string RoomName { get; set; } = string.Empty;

        public byte[] EncryptMessage(byte[] plaintext)
        {
            var iv = RandomNumberGenerator.GetBytes(12);
            var key = string.IsNullOrEmpty(RoomName) ? aesKey : DeriveRoomKey();

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

        public byte[] DecryptMessage(byte[] encryptedData)
        {
            if (encryptedData.Length < 12 + 16)
                throw new InvalidDataException("Invalid encrypted data");

            var iv = encryptedData.AsSpan(0, 12).ToArray();
            var authTag = encryptedData.AsSpan(encryptedData.Length - 16, 16).ToArray();
            var ciphertext = encryptedData.AsSpan(12, encryptedData.Length - 12 - 16).ToArray();
            var key = string.IsNullOrEmpty(RoomName) ? aesKey : DeriveRoomKey();

            using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);
            var plaintext = new byte[ciphertext.Length];

            aesGcm.Decrypt(iv, ciphertext, authTag, plaintext);

            return plaintext;
        }

        private byte[] DeriveRoomKey()
        {
            return HKDF.DeriveKey(
                ikm: aesKey,
                salt: Encoding.UTF8.GetBytes(RoomName),
                info: "room-key"u8.ToArray(),
                outputLength: 32,
                hashAlgorithmName: HashAlgorithmName.SHA256);
        }

        void IDisposable.Dispose()
        {
            if (disposed) return;
            Array.Clear(aesKey);
            disposed = true;
        }
    }
}
