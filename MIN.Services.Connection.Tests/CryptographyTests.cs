using System.Security.Cryptography;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Cryptographing;
using MIN.Services.Services;
using Xunit;

namespace MIN.Services.Connection.Tests
{
    /// <summary>
    /// Тесты для <see cref="ICryptoProvider"/>
    /// </summary>
    public class CryptographyTests
    {
        public CryptographyTests()
        {

        }

        [Fact]
        public async Task Ecdh_WithDerBase64_TwoProviders_ShouldMatch()
        {
            // Arrange: два независимых провайдера (имитация разных ПК)
            var providerA = new KeyProvider(new LoggerProvider());
            var providerB = new KeyProvider(new LoggerProvider());

            var keysA = await providerA.GetLocalKeysAsync();
            var keysB = await providerB.GetLocalKeysAsync();

            // Act: вычисляем shared secret в обе стороны
            var secretA = await providerA.ComputeSharedSecretAsync(keysB.EcdhPublicKeyPem);
            var secretB = await providerB.ComputeSharedSecretAsync(keysA.EcdhPublicKeyPem);

            // Assert
            Assert.Equal(secretA, secretB); // ✅ Должно пройти!
            Assert.Equal(32, secretA.Length);

            // Дополнительно: проверка шифрования
            using var aesA = new AesGcm(secretA, tagSizeInBytes: 16);
            using var aesB = new AesGcm(secretB, tagSizeInBytes: 16);

            var plaintext = "Test"u8.ToArray();
            var iv = RandomNumberGenerator.GetBytes(12);
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[16];

            aesA.Encrypt(iv, plaintext, ciphertext, tag);

            var decrypted = new byte[ciphertext.Length];
            aesB.Decrypt(iv, ciphertext, tag, decrypted); // ✅ Не должно выбросить

            Assert.Equal(plaintext, decrypted);
        }
    }
}
