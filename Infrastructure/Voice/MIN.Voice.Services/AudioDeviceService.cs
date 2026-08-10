using System.Runtime.InteropServices;
using MIN.Voice.Services.Contacts.Interfaces;
using MIN.Voice.Services.Contacts.Models;
using NAudio;
using NAudio.Wave;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IAudioDeviceService"/>
public class AudioDeviceService : IAudioDeviceService
{
    IReadOnlyList<AudioDeviceInfo> IAudioDeviceService.GetInputDevices()
    {
        var result = new List<AudioDeviceInfo>();
        for (var i = 0; i < WaveInEvent.DeviceCount; i++)
        {
            result.Add(new AudioDeviceInfo(i, WaveInEvent.GetCapabilities(i).ProductName));
        }
        return result;
    }

    IReadOnlyList<AudioDeviceInfo> IAudioDeviceService.GetOutputDevices()
    {
        // TODO: maybe some day this hacky code will be the same as the one above (idk why on output wave there are no device infos)
        var result = new List<AudioDeviceInfo>();
        var count = WaveInterop.waveOutGetNumDevs();
        for (var i = 0; i < count; i++)
        {
            var caps = new WaveOutCapabilities();
            MmException.Try(WaveInterop.waveOutGetDevCaps(i, out caps, Marshal.SizeOf(caps)), "waveOutGetDevCaps");
            result.Add(new AudioDeviceInfo(i, caps.ProductName));
        }
        return result;
    }
}
