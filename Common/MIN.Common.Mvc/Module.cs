using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MIN.Common.Mvc;

/// <summary>
/// Модуль регистрации зависимостей
/// </summary>
public abstract class Module
{
    /// <summary>
    /// Конфигурирует модули
    /// </summary>
    public void Configure(IServiceCollection services)
    {
        Load(services);
    }

    /// <summary>
    /// Загрузка зависимостей
    /// </summary>
    protected abstract void Load(IServiceCollection services);

    /// <summary>
    /// Gets the assembly in which the concrete module type is located. To avoid bugs whereby deriving from a module will
    /// change the target assembly, this property can only be used by modules that inherit directly from
    /// <see cref="Module"/>.
    /// </summary>
    protected virtual Assembly ThisAssembly
    {
        get
        {
            var thisType = GetType();
            var baseType = thisType.BaseType;
            if (baseType != typeof(Module))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "This assembly unavailable {0} of {1}", thisType, baseType));
            }

            return thisType.Assembly;
        }
    }
}
