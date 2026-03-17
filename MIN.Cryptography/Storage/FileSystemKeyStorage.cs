using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Cryptography.Contracts.Interfaces.Storage;
using MIN.Cryptography.Contracts.Models;

namespace MIN.Cryptography.Storage
{
    /// <summary>
    /// Реализация хранилища ключей на основе файловой системы с использованием JSON
    /// </summary>
    public sealed class FileSystemKeyStorage : IKeyStorage, IDisposable
    {
        private readonly string keysPath;
        private readonly string partnersPath;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly SemaphoreSlim localKeyLock = new(1, 1);
        private readonly SemaphoreSlim partnersLock = new(1, 1);
        private bool disposed;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="FileSystemKeyStorage"/>
        /// </summary>
        public FileSystemKeyStorage(string baseDirectory, JsonSerializerOptions? jsonOptions = null)
        {
            Directory.CreateDirectory(baseDirectory);
            keysPath = Path.Combine(baseDirectory, "keys.json");
            partnersPath = Path.Combine(baseDirectory, "partners.json");
            this.jsonOptions = jsonOptions ?? new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<KeyPair?> LoadLocalKeyPairAsync(CancellationToken cancellationToken = default)
        {
            await localKeyLock.WaitAsync(cancellationToken);
            try
            {
                if (!File.Exists(keysPath))
                {
                    return null;
                }

                var json = await File.ReadAllTextAsync(keysPath, cancellationToken);
                return JsonSerializer.Deserialize<KeyPair>(json, jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("Local key file is corrupted", ex);
            }
            finally
            {
                localKeyLock.Release();
            }
        }

        public async Task SaveLocalKeyPairAsync(KeyPair keyPair, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(keyPair);

            await localKeyLock.WaitAsync(cancellationToken);
            try
            {
                var json = JsonSerializer.Serialize(keyPair, jsonOptions);
                await File.WriteAllTextAsync(keysPath, json, cancellationToken);
            }
            finally
            {
                localKeyLock.Release();
            }
        }

        public async Task<Dictionary<Guid, byte[]>> LoadPartnerPublicKeysAsync(CancellationToken cancellationToken = default)
        {
            await partnersLock.WaitAsync(cancellationToken);
            try
            {
                if (!File.Exists(partnersPath))
                {
                    return new Dictionary<Guid, byte[]>();
                }

                var json = await File.ReadAllTextAsync(partnersPath, cancellationToken);
                var stringDict = JsonSerializer.Deserialize<Dictionary<string, string>>(json, jsonOptions);
                if (stringDict == null)
                {
                    return new Dictionary<Guid, byte[]>();
                }

                var result = new Dictionary<Guid, byte[]>();
                foreach (var kvp in stringDict)
                {
                    if (Guid.TryParse(kvp.Key, out var guid))
                    {
                        result[guid] = Convert.FromBase64String(kvp.Value);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Failed to load partners file", ex);
            }
            finally
            {
                partnersLock.Release();
            }
        }

        public async Task SavePartnerPublicKeyAsync(Guid partnerId, byte[] publicKey, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(publicKey);

            await partnersLock.WaitAsync(cancellationToken);
            try
            {
                var partners = await LoadPartnerPublicKeysAsync(cancellationToken);
                partners[partnerId] = publicKey;

                var stringDict = partners.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => Convert.ToBase64String(kvp.Value));

                var json = JsonSerializer.Serialize(stringDict, jsonOptions);
                await File.WriteAllTextAsync(partnersPath, json, cancellationToken);
            }
            finally
            {
                partnersLock.Release();
            }
        }

        public async Task<byte[]?> LoadPartnerPublicKeyAsync(Guid partnerId, CancellationToken cancellationToken = default)
        {
            var partners = await LoadPartnerPublicKeysAsync(cancellationToken);
            return partners.TryGetValue(partnerId, out var key) ? key : null;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            localKeyLock.Dispose();
            partnersLock.Dispose();
            disposed = true;
        }
    }
}
