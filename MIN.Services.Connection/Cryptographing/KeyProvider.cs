using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Models.Cryptographing;

namespace MIN.Services.Connection.Cryptographing
{
    /// <inheritdoc cref="IKeyProvider"/>
    public class KeyProvider : IKeyProvider
    {
        private readonly string _keysPath;
        private readonly string _partnersPath;
        private KeyPair? _cachedKeys;

        public KeyProvider()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "MIN-Messenger");
            Directory.CreateDirectory(dir);

            _keysPath = Path.Combine(dir, "keys.json");
            _partnersPath = Path.Combine(dir, "partners.json");
        }

        public async Task<KeyPair> GetLocalKeysAsync()
        {
            if (_cachedKeys != null) return _cachedKeys;

            if (File.Exists(_keysPath))
            {
                var json = await File.ReadAllTextAsync(_keysPath);
                _cachedKeys = JsonSerializer.Deserialize<KeyPair>(json)
                    ?? throw new InvalidDataException("Failed to load keys");
            }
            else
            {
                _cachedKeys = await GenerateNewKeysAsync();
                await SaveKeysAsync(_cachedKeys);
            }

            return _cachedKeys;
        }

        private async Task<KeyPair> GenerateNewKeysAsync()
        {
            using var rsa = RSA.Create(4096);

            // Генерируем случайный AES-ключ (256 бит)
            var aesKey = RandomNumberGenerator.GetBytes(32);

            // Генерируем соль (32 байта, не секрет)
            var salt = RandomNumberGenerator.GetBytes(32);

            return new KeyPair
            {
                PublicKeyPem = rsa.ExportRSAPublicKeyPem(),
                EncryptedPrivateKeyPem = Protect(rsa.ExportRSAPrivateKeyPem()),
                EncryptedAesKey = Protect(Convert.ToBase64String(aesKey)),
                SaltHex = Convert.ToHexString(salt),
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<RSA> GetRsaPrivateKeyAsync()
        {
            var keys = await GetLocalKeysAsync();
            var decryptedPem = Unprotect(keys.EncryptedPrivateKeyPem);

            var rsa = RSA.Create();
            rsa.ImportFromPem(decryptedPem);
            return rsa;
        }

        public async Task<byte[]> GetAesKeyAsync()
        {
            var keys = await GetLocalKeysAsync();
            var decryptedBase64 = Unprotect(keys.EncryptedAesKey);
            return Convert.FromBase64String(decryptedBase64);
        }

        public async Task<byte[]> GetSaltAsync()
        {
            var keys = await GetLocalKeysAsync();
            return Convert.FromHexString(keys.SaltHex);
        }

        // 🔐 DPAPI-обёртки
        private string Protect(string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        private string Unprotect(string protectedBase64)
        {
            var protectedBytes = Convert.FromBase64String(protectedBase64);
            var bytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }

        private async Task SaveKeysAsync(KeyPair keys)
        {
            var json = JsonSerializer.Serialize(keys, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_keysPath, json);
        }

        public async Task SavePartnerPublicKeyAsync(string partnerId, string publicKeyPem)
        {
            var partners = await LoadPartnersAsync();
            partners[partnerId] = publicKeyPem;
            await SavePartnersAsync(partners);
        }

        public async Task<string?> GetPartnerPublicKeyAsync(string partnerId)
        {
            var partners = await LoadPartnersAsync();
            return partners.TryGetValue(partnerId, out var key) ? key : null;
        }

        private async Task<Dictionary<string, string>> LoadPartnersAsync()
        {
            if (!File.Exists(_partnersPath)) return new();
            var json = await File.ReadAllTextAsync(_partnersPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }

        private async Task SavePartnersAsync(Dictionary<string, string> partners)
        {
            var json = JsonSerializer.Serialize(partners, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_partnersPath, json);
        }
    }
}
