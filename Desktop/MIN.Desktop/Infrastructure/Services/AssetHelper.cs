using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Platform;

namespace MIN.Desktop.Infrastructure.Services;

/// <summary>
/// Помошник с assetами
/// </summary>
public class AssetHelper
{
    private readonly static string assemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new Exception("Unable to get Assembly name");
    private readonly static Dictionary<string, Uri> assetPathCache = [];

    /// <summary>
    /// Получить asset
    /// </summary>
    public static Uri? GetFullAssetPath(string assetPath)
    {
        if (assetPathCache.TryGetValue(assetPath, out Uri? fullPath))
        {
            return fullPath;
        }

        Uri uri = assetPath.StartsWith("avares://") ? new Uri(assetPath) : new Uri($"avares://{assemblyName}{assetPath}");
        if (!AssetLoader.Exists(uri) && !Design.IsDesignMode)
        {
            return assetPathCache[assetPath] = null!;
        }
        return assetPathCache[assetPath] = uri;
    }
}
