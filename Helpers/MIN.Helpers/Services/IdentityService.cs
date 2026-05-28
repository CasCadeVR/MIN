using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Core.Entities.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Helpers.Services;

/// <inheritdoc cref="IIdentityService"/>
public sealed class IdentityService : IIdentityService
{
    private readonly string participantIdPath;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly SemaphoreSlim localKeyLock = new(1, 1);
    private readonly SemaphoreSlim cacheLock = new(1, 1);

    private ParticipantInfo? currentParticipant;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="IdentityService"/>
    /// </summary>
    public IdentityService(IAppDataProvider appDataProvider)
    {
        participantIdPath = Path.Combine(appDataProvider.BaseDirectory, "uuid.json");

        jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    IParticipantData IIdentityService.SelfParticipant => ResolveParticipant();

    /// <inheritdoc />
    void IIdentityService.SetParticipant(IParticipantData participantData)
    {
        if (currentParticipant != null)
        {
            currentParticipant.Name = participantData.Name;
        }
    }

    /// <inheritdoc />
    void IIdentityService.ResetParticipant()
    {
        currentParticipant = null;
    }

    private IParticipantData ResolveParticipant()
    {
        if (currentParticipant != null)
        {
            return currentParticipant;
        }

        cacheLock.Wait();
        try
        {
            if (currentParticipant != null)
            {
                return currentParticipant;
            }

            currentParticipant = new ParticipantInfo
            {
                Id = Guid.NewGuid(),
                Name = "Ты"
            };

            // TODO: Когда нибудь заменить на LoadParticipantId()

            //if (participantId == Guid.Empty)
            //{
            //    currentParticipant.Id = Guid.NewGuid();
            //    SaveBroadcastAddressesAsync(currentParticipant.Id);
            //}

            return currentParticipant;
        }
        finally
        {
            cacheLock.Release();
        }
    }

    private Guid LoadParticipantId()
    {
        localKeyLock.Wait();
        try
        {
            if (!File.Exists(participantIdPath))
            {
                return Guid.Empty;
            }

            var json = File.ReadAllText(participantIdPath);
            return JsonSerializer.Deserialize<Guid>(json, jsonOptions);
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

    private void SaveBroadcastAddressesAsync(Guid participantId)
    {
        localKeyLock.Wait();
        try
        {
            var json = JsonSerializer.Serialize(participantId, jsonOptions);
            File.WriteAllText(participantIdPath, json);
        }
        finally
        {
            localKeyLock.Release();
        }
    }
}
