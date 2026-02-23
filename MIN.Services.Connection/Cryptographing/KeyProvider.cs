using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Models.Cryptographing;
using MIN.Services.Contracts.Interfaces;

namespace MIN.Services.Connection.Cryptographing
{
    /// <inheritdoc cref="IKeyProvider"/>
    public class KeyProvider : IKeyProvider
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly ILoggerProvider logger;
        private readonly string keysPath;
        private readonly string partnersPath;
        private KeyPair? cachedKeys;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private bool disposed;

        public KeyProvider(ILoggerProvider logger)
        {
            // Храним ключи в %AppData%\MIN-Messenger\
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "MIN-Messenger");
            Directory.CreateDirectory(dir);
            this.logger = logger;

            keysPath = Path.Combine(dir, "keys.json");
            partnersPath = Path.Combine(dir, "partners.json");
        }

        public async Task<KeyPair> GetLocalKeysAsync()
        {
            if (cachedKeys != null) return cachedKeys;

            await _lock.WaitAsync();
            try
            {
                if (cachedKeys != null) return cachedKeys;

                if (File.Exists(keysPath))
                {
                    var json = await File.ReadAllTextAsync(keysPath);
                    cachedKeys = JsonSerializer.Deserialize<KeyPair>(json, jsonOptions)
                        ?? throw new InvalidDataException("Failed to deserialize keys");
                }
                else
                {
                    cachedKeys = await GenerateNewKeysAsync();
                    await SaveKeysAsync(cachedKeys);
                }

                return cachedKeys;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<ECDiffieHellman> GetEcdhPrivateKeyAsync()
        {
            var keys = await GetLocalKeysAsync();
            var decryptedPem = Unprotect(keys.EncryptedEcdhPrivateKeyPem);

            var ecdh = ECDiffieHellman.Create();
            ecdh.ImportFromPem(NormalizePem(decryptedPem));
            return ecdh;
        }

        public async Task<RSA?> GetRsaPrivateKeyAsync()
        {
            var keys = await GetLocalKeysAsync();

            if (string.IsNullOrEmpty(keys.EncryptedRsaPrivateKeyPem))
                return null;

            var decryptedPem = Unprotect(keys.EncryptedRsaPrivateKeyPem);

            var rsa = RSA.Create();
            rsa.ImportFromPem(NormalizePem(decryptedPem));
            return rsa;
        }

        public async Task<byte[]> ComputeSharedSecretAsync(string partnerEcdhPublicKeyPem)
        {
            logger.Log($"🔐 My ECDH public (first 32 chars): {await GetEcdhPrivateKeyAsync()}");
            logger.Log($"🔐 Partner ECDH public (first 32 chars): {partnerEcdhPublicKeyPem.Substring(0, 32)}...");

            using var myEcdh = await GetEcdhPrivateKeyAsync();

            using var partnerEcdh = ECDiffieHellman.Create();
            partnerEcdh.ImportFromPem(partnerEcdhPublicKeyPem);

            // Вычисляем общий секрет через ECDH
            var sharedSecret = myEcdh.DeriveKeyFromHash(
                partnerEcdh.PublicKey,
                HashAlgorithmName.SHA256,
                null, // salt для HKDF внутри (не нужен)
                null  // другие параметры
            );

            logger.Log($"🔐 Computed shared secret (first 16 bytes): {Convert.ToHexString(sharedSecret.AsSpan(0, 16))}");

            // Деривируем финальный AES-ключ через HKDF с нашей солью
            var salt = await GetSaltAsync();
            return HKDF.DeriveKey(
                ikm: sharedSecret,
                salt: salt,
                info: "encryption"u8.ToArray(),
                outputLength: 32, // AES-256
                hashAlgorithmName: HashAlgorithmName.SHA256
            );
        }

        public async Task SavePartnerPublicKeyAsync(Guid partnerId, string ecdhPublicKeyPem)
        {
            var partners = await LoadPartnersAsync();
            partners[partnerId.ToString()] = ecdhPublicKeyPem;
            await SavePartnersAsync(partners);
        }

        public async Task<string?> GetPartnerPublicKeyAsync(Guid partnerId)
        {
            var partners = await LoadPartnersAsync();
            return partners.TryGetValue(partnerId.ToString(), out var key) ? key : null;
        }

        public async Task<byte[]> GetSaltAsync()
        {
            var keys = await GetLocalKeysAsync();
            return Convert.FromHexString(keys.SaltHex);
        }

        private async Task<KeyPair> GenerateNewKeysAsync()
        {
            using var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
            using var rsa = RSA.Create(2048);

            var salt = RandomNumberGenerator.GetBytes(32);

            return new KeyPair
            {
                EcdhPublicKeyPem = NormalizePem(ecdh.ExportSubjectPublicKeyInfoPem()),
                EncryptedEcdhPrivateKeyPem = Protect(ecdh.ExportPkcs8PrivateKeyPem()),
                RsaPublicKeyPem = NormalizePem(rsa.ExportSubjectPublicKeyInfoPem()),
                EncryptedRsaPrivateKeyPem = Protect(rsa.ExportPkcs8PrivateKeyPem()),

                SaltHex = Convert.ToHexString(salt),
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task SaveKeysAsync(KeyPair keys)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(keys, options);
            await File.WriteAllTextAsync(keysPath, json);
        }

        private async Task<Dictionary<string, string>> LoadPartnersAsync()
        {
            if (!File.Exists(partnersPath))
                return new Dictionary<string, string>();

            var json = await File.ReadAllTextAsync(partnersPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();
        }

        private async Task SavePartnersAsync(Dictionary<string, string> partners)
        {
            var json = JsonSerializer.Serialize(partners, jsonOptions);
            await File.WriteAllTextAsync(partnersPath, json);
        }

        /// <summary>
        /// Защитить данные через DPAPI (привязка к пользователю Windows)
        /// </summary>
        private string Protect(string plainText)
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

        /// <summary>
        /// Расшифровать данные через DPAPI
        /// </summary>
        private string Unprotect(string protectedBase64)
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

        public void Dispose()
        {
            if (disposed) return;

            _lock.Dispose();
            disposed = true;
        }

        private static string NormalizePem(string pem)
        {
            if (string.IsNullOrWhiteSpace(pem)) return pem;

            return pem
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Trim();
        }
    }
}
