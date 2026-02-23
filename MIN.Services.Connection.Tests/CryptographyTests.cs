using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Cryptographing;
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
        public async Task Ecdh_KeyExchange_TwoIndependentProviders_ShouldProduceSameSharedSecret()
        {
            // Arrange: два независимых KeyProvider (имитация разных ПК)
            var providerA = new KeyProvider(); // У каждого свой keys.json
            var providerB = new KeyProvider();

            // Генерируем ключи
            var keysA = await providerA.GetLocalKeysAsync();
            var keysB = await providerB.GetLocalKeysAsync();

            // Act: вычисляем shared secret в обе стороны
            var secretA = await providerA.ComputeSharedSecretAsync(keysB.EcdhPublicKeyPem);
            var secretB = await providerB.ComputeSharedSecretAsync(keysA.EcdhPublicKeyPem);

            // Assert: секреты должны быть идентичны
            Assert.Equal(secretA, secretB);
            Assert.Equal(32, secretA.Length); // AES-256

            // Дополнительно: проверим шифрование/расшифрование
            using var aesA = new AesGcm(secretA, tagSizeInBytes: 16);
            using var aesB = new AesGcm(secretB, tagSizeInBytes: 16);

            var plaintext = "Test message"u8.ToArray();
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
