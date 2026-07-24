using System;
using Avalonia.Controls;

namespace MIN.Desktop.Contracts.Models;

/// <summary>
/// Маппинг для регистрации DI для окон
/// </summary>
public record WindowMapping(Type ViewModelType, Func<Type, Window> WindowFactory);
