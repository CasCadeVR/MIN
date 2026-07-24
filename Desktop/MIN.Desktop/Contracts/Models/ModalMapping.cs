using System;
using MIN.Desktop.Views.Base;

namespace MIN.Desktop.Contracts.Models;

/// <summary>
/// Маппинг для регистрации DI для диалогов
/// </summary>
public record ModalMapping(Type ViewModelType, Func<Type, ModalViewBase> WindowFactory);
