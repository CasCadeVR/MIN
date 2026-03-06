using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Models.Cryptographing;

namespace MIN.Services.Connection.Cryptographing
{
    /// <inheritdoc cref="IKeyProvider"/>
    public class KeyProvider : IKeyProvider
    {
        private readonly static JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly string keysPath;
        private readonly string partnersPath;
        private KeyPair? cachedKeys;
        private readonly SemaphoreSlim @lock = new(1, 1);
        private bool disposed;

        public KeyProvider()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = Path.Combine(appData, "MIN-Messenger");
            Directory.CreateDirectory(dir);

            keysPath = Path.Combine(dir, "keys.json");
            partnersPath = Path.Combine(dir, "partners.json");
        }

        public async Task<KeyPair> GetLocalKeysAsync()
        {
            if (cachedKeys != null)
            {
                return cachedKeys;
            }

            await @lock.WaitAsync();
            try
            {
                if (cachedKeys != null)
                {
                    return cachedKeys;
                }

                if (File.Exists(keysPath))
                {
                    var json = await File.ReadAllTextAsync(keysPath);
                    cachedKeys = JsonSerializer.Deserialize<KeyPair>(json, jsonOptions)
                        ?? throw new InvalidDataException("Failed to deserialize keys");
                }
                else
                {
                    cachedKeys = GenerateNewKeys();
                    await SaveKeysAsync(cachedKeys);
                }

                return cachedKeys;
            }
            finally
            {
                @lock.Release();
            }
        }

        public async Task<ECDiffieHellman> GetEcdhPrivateKeyAsync()
        {
            var keys = await GetLocalKeysAsync();
            var decryptedPem = Unprotect(keys.EncryptedEcdhPrivateKeyPem);

            var ecdh = ECDiffieHellman.Create();
            ecdh.ImportFromPem(decryptedPem);
            return ecdh;
        }

        public async Task<byte[]> ComputeSharedSecretAsync(string partnerEcdhPublicKeyDerBase64)
        {
            var partnerPublicKeyBytes = Convert.FromBase64String(partnerEcdhPublicKeyDerBase64);
            using var myEcdh = await GetEcdhPrivateKeyAsync();

            var myPublicKeyBytes = myEcdh.ExportSubjectPublicKeyInfo();
            var myPublicKeyBase64 = Convert.ToBase64String(myPublicKeyBytes);

            using var partnerEcdh = ECDiffieHellman.Create();
            partnerEcdh.ImportSubjectPublicKeyInfo(partnerPublicKeyBytes, out _);

            var sharedSecret = myEcdh.DeriveKeyFromHash(
                partnerEcdh.PublicKey,
                HashAlgorithmName.SHA256,
                null, null);

            var sharedSecretHex = Convert.ToHexString(sharedSecret);

            var aesKey = HKDF.DeriveKey(
                ikm: sharedSecret,
                salt: null,
                info: "encryption"u8.ToArray(),
                outputLength: 32,
                hashAlgorithmName: HashAlgorithmName.SHA256);

            var aesKeyHex = Convert.ToHexString(aesKey);

            return aesKey;
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

        private KeyPair GenerateNewKeys()
        {
            using var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);

            return new KeyPair
            {
                EcdhPublicKeyPem = ecdh.ExportSubjectPublicKeyInfoPem(),
                EncryptedEcdhPrivateKeyPem = Protect(ecdh.ExportPkcs8PrivateKeyPem()),
                EcdhPublicKeyDerBase64 = Convert.ToBase64String(ecdh.ExportSubjectPublicKeyInfo()),
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task SaveKeysAsync(KeyPair keys)
        {
            var json = JsonSerializer.Serialize(keys, jsonOptions);
            await File.WriteAllTextAsync(keysPath, json);
        }

        private async Task<Dictionary<string, string>> LoadPartnersAsync()
        {
            if (!File.Exists(partnersPath))
            {
                return new Dictionary<string, string>();
            }

            var json = await File.ReadAllTextAsync(partnersPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json, jsonOptions)
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
            if (disposed)
            {
                return;
            }

            @lock.Dispose();
            disposed = true;
        }
    }
}
