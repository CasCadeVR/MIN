using System.Security.Cryptography;
using System.Text;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;

namespace MIN.Services.Connection.Cryptographing
{
    public class CryptoProvider : ICryptoProvider
    {
        private readonly IKeyProvider keyProvider;
        private RSA? cachedRsa;
        private byte[]? cachedAesKey;

        public CryptoProvider(IKeyProvider keyProvider)
        {
            this.keyProvider = keyProvider;
        }

        private async Task<RSA> GetRsaAsync() =>
            cachedRsa ??= await keyProvider.GetRsaPrivateKeyAsync();

        private async Task<byte[]> GetDerivedAesKeyAsync()
        {
            if (cachedAesKey != null) return cachedAesKey;

            var masterKey = await keyProvider.GetAesKeyAsync();
            var salt = await keyProvider.GetSaltAsync();

            cachedAesKey = HKDF.DeriveKey(
                ikm: masterKey,
                salt: salt,
                info: Encoding.UTF8.GetBytes("encryption"),
                outputLength: 32, // AES-256
                hashAlgorithmName: HashAlgorithmName.SHA256);

            return cachedAesKey;
        }

        public async Task<byte[]> EncryptMessageAsync(byte[] plaintext)
        {
            var aesKey = await GetDerivedAesKeyAsync();
            var rsa = await GetRsaAsync();

            // Генерируем уникальный IV для этого сообщения
            var iv = RandomNumberGenerator.GetBytes(12);

            // Шифруем через AesGcm
            using var aesGcm = new AesGcm(aesKey, tagSizeInBytes: 16);
            var ciphertext = new byte[plaintext.Length];
            var authTag = new byte[16];

            aesGcm.Encrypt(iv, plaintext, ciphertext, authTag);

            // Подписываем ciphertext своим приватным ключом (для аутентификации отправителя)
            var signature = rsa.SignData(
                ciphertext,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            // Формат пакета: [IV:12][Ciphertext:N][AuthTag:16][Signature:512]
            using var ms = new MemoryStream();
            ms.Write(iv, 0, 12);
            ms.Write(ciphertext, 0, ciphertext.Length);
            ms.Write(authTag, 0, 16);
            ms.Write(signature, 0, signature.Length);

            return ms.ToArray();
        }

        public async Task<byte[]> DecryptMessageAsync(byte[] encryptedData)
        {
            var aesKey = await GetDerivedAesKeyAsync();
            var rsa = await GetRsaAsync();

            // Парсим формат: [IV:12][Ciphertext:N][AuthTag:16][Signature:512]
            var signatureSize = rsa.KeySize / 8; // 512 для RSA-4096
            var minSize = 12 + 16 + signatureSize;

            if (encryptedData.Length < minSize)
                throw new InvalidDataException("Encrypted data too short");

            var iv = encryptedData.AsSpan(0, 12).ToArray();
            var signature = encryptedData.AsSpan(encryptedData.Length - signatureSize, signatureSize).ToArray();
            var authTag = encryptedData.AsSpan(encryptedData.Length - signatureSize - 16, 16).ToArray();
            var ciphertext = encryptedData.AsSpan(12, encryptedData.Length - 12 - 16 - signatureSize).ToArray();

            // Проверяем RSA-подпись (аутентификация отправителя)
            if (!rsa.VerifyData(ciphertext, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                throw new CryptographicException("Invalid signature: message may be tampered");

            // Расшифровываем через AesGcm (автоматическая проверка целостности через authTag)
            using var aesGcm = new AesGcm(aesKey, tagSizeInBytes: 16);
            var plaintext = new byte[ciphertext.Length];

            // Если authTag не совпадёт — выбросит CryptographicException
            aesGcm.Decrypt(iv, ciphertext, authTag, plaintext);

            return plaintext;
        }
    }
}
