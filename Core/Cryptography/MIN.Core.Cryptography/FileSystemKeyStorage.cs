using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Cryptography.Contracts.Models;

namespace MIN.Core.Cryptography;

/// <summary>
/// <see cref="IKeyStorage"/> на основе файловой системы
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
    public FileSystemKeyStorage(string baseDirectory)
    {
        Directory.CreateDirectory(baseDirectory);
        keysPath = Path.Combine(baseDirectory, "keys.json");
        partnersPath = Path.Combine(baseDirectory, "partners.json");
        jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    async Task<KeyPair?> IKeyStorage.LoadLocalKeyPairAsync(CancellationToken cancellationToken)
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

    async Task IKeyStorage.SaveLocalKeyPairAsync(KeyPair keyPair, CancellationToken cancellationToken)
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

    /// <inheritdoc />
    public async Task<Dictionary<Guid, byte[]>> LoadPartnerPublicKeysAsync(CancellationToken cancellationToken = default)
    {
        await partnersLock.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(partnersPath))
            {
                return [];
            }

            var json = await File.ReadAllTextAsync(partnersPath, cancellationToken);
            var stringDict = JsonSerializer.Deserialize<Dictionary<string, string>>(json, jsonOptions);
            if (stringDict == null)
            {
                return [];
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

    async Task IKeyStorage.SavePartnerPublicKeyAsync(Guid partnerId, byte[] publicKey, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(publicKey);

        var partners = await LoadPartnerPublicKeysAsync(cancellationToken);
        partners[partnerId] = publicKey;

        var stringDict = partners.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => Convert.ToBase64String(kvp.Value));

        var json = JsonSerializer.Serialize(stringDict, jsonOptions);
        await File.WriteAllTextAsync(partnersPath, json, cancellationToken);
    }

    async Task<byte[]?> IKeyStorage.LoadPartnerPublicKeyAsync(Guid partnerId, CancellationToken cancellationToken)
    {
        var partners = await LoadPartnerPublicKeysAsync(cancellationToken);
        return partners.TryGetValue(partnerId, out var key) ? key : null;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
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
