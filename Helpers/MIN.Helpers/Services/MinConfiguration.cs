using Microsoft.Extensions.Configuration;

namespace MIN.Helpers.Services;

/// <summary>
/// Конфигурирование проекта
/// </summary>
public sealed class MinConfiguration
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MinConfiguration"/>
    /// </summary>
    public MinConfiguration(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
}
