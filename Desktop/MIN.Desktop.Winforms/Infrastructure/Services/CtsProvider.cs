using MIN.Desktop.Contracts.Interfaces;

namespace MIN.Desktop.Infrastructure.Services;

/// <inheritdoc cref="ICtsProvider"/>
public class CtsProvider : ICtsProvider
{
    private readonly CancellationTokenSource cts = new();

    CancellationTokenSource ICtsProvider.AppCts => cts;
}
