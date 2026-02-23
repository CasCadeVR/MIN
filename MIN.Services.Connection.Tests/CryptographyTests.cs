using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Cryptographing;
using Moq;
using Xunit;

namespace MIN.Services.Connection.Tests
{
    /// <summary>
    /// Тесты для <see cref="ICryptoProvider"/>
    /// </summary>
    public class CryptographyTests
    {
        private readonly Mock<IKeyProvider> mockKeyProvider;
        private readonly byte[] testKey;

        public CryptographyTests()
        {
            mockKeyProvider = new Mock<IKeyProvider>();
            testKey = RandomNumberGenerator.GetBytes(32); // 256 бит

            mockKeyProvider
                .Setup(k => k.GetOrCreateAesKey())
                .Returns(testKey);
        }

        [Fact]
        public void EncryptDecrypt_RoundTrip_ShouldReturnOriginalPlaintext()
        {
            // Arrange
            var message = "Nail sigma";
            var crypto = new CryptoProvider(mockKeyProvider.Object);
            var original = Encoding.UTF8.GetBytes(message);

            // Act
            var encrypted = crypto.EncryptMessage(original);
            var decrypted = crypto.DecryptMessage(encrypted);

            // Assert
            decrypted.Should().BeEquivalentTo(original);
            Encoding.UTF8.GetString(decrypted).Should().BeEquivalentTo(message);
        }

        [Fact]
        public void Decrypt_TamperedData_ShouldThrowCryptographicException()
        {
            // Arrange
            var crypto = new CryptoProvider(mockKeyProvider.Object);
            var original = "Secret message"u8.ToArray();
            var encrypted = crypto.EncryptMessage(original);

            // Act: портим один байт в ciphertext (не в IV или authTag)
            encrypted[15] ^= 0xFF; // XOR для изменения бита

            var result = () => crypto.DecryptMessage(encrypted);

            // Assert
            result.Should().Throw<CryptographicException>();
        }
    }
}
