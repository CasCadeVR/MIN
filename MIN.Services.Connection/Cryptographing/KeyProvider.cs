using System.Security.Cryptography;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;

namespace MIN.Services.Connection.Cryptographing
{
    /// <inheritdoc cref="IKeyProvider"/>
    public class KeyProvider : IKeyProvider
    {
        private readonly string keyPath;

        public KeyProvider()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "MIN-Messenger");
            Directory.CreateDirectory(dir);
            keyPath = Path.Combine(dir, "master.key");
        }

        /// <summary>
        /// Получить AES-ключ (32 байта). Если нет — сгенерировать и сохранить.
        /// Ключ зашифрован через DPAPI (привязка к пользователю Windows).
        /// </summary>
        public byte[] GetOrCreateAesKey()
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("К сожалению только Windows");
            }

            if (File.Exists(keyPath))
            {
                var protectedKeyBytes = File.ReadAllBytes(keyPath);
                return ProtectedData.Unprotect(protectedKeyBytes, null, DataProtectionScope.CurrentUser);
            }

            var newKey = RandomNumberGenerator.GetBytes(32);

            var protectedBytes = ProtectedData.Protect(newKey, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(keyPath, protectedBytes);

            Array.Clear(newKey);

            return ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
        }
    }
}
