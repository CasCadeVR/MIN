# Single-Form Interface Design

## Overview

Реорганизация WinForms UI с отдельных форм на single-form интерфейс с переключаемыми панелями (panels) внутри одной главной формы.

## Goals

1. Одна главная форма (MainForm) с переключаемыми панелями
2. Side panel: боковая навигация с возможностью масштабирования
3. Main panel: основной контент (Discovery, Chat как overlay)
4. Переиспользование сервисов через Feature Services
5. Runtime параметры для панелей (roomId, endpoint)

## Architecture

### Panel Types

| Panel | Type | Description |
|-------|------|-------------|
| MainSidePanelView | Side (fixed) | Главное меню |
| DiscoveryPanelView | Main или Side | Поиск комнат |
| SettingsPanelView | Side | Настройки (overlay) |
| ChatPanelView | Main (overlay) | Чат комнаты |
| LoadingPanelView | Main | Экран загрузки |

### Navigation

```
MainForm
├── sidePanel (всегда MainSidePanelView)
│   ├── Discovery
│   └── Settings (+ кнопка "Назад")
└── mainPanel (переключается)
    ├── DiscoveryPanelView (main)
    ├── ChatPanelView (overlay, не уничтожает discovery при переключении)
    └── LoadingPanelView
```

### Feature Services

```csharp
public interface IDiscoveryPanelService
{
    IDiscoveryService DiscoveryService { get; }
    IRoomConnector RoomConnector { get; }
    IRoomHoster RoomHoster { get; }
    IRoomStore RoomStore { get; }
    IRoomFactory RoomFactory { get; }
    IEventBus EventBus { get; }
    ISettingsProvider SettingsProvider { get; }
    ILocalNetworkComputerProvider ComputerProvider { get; }
    IIdentityService IdentityService { get; }
    ParticipantInfo LocalParticipant { get; }
    SynchronizationContext UiContext { get; }
    CancellationTokenSource Cts { get; }
}

public interface IChatPanelService
{
    IChatService ChatService { get; }
    IEventBus EventBus { get; }
    INotificationService NotificationService { get; }
    ILoggerProvider Logger { get; }
    IIdentityService IdentityService { get; }
    Room CurrentRoom { get; set; }
    IEndpoint Endpoint { get; set; }
}
```

### Resolution

Панели регистрируются в IoC контейнере через constructor injection.

Для runtime параметров:
- Панель создаётся через DI с сервисами
- Runtime параметры устанавливаются через Initialize(room, endpoint)

## Side Panel Scale

MainSidePanelView содержит динамический список кнопок:
- Каждая кнопка связана с соответствующей Side panel
- При нажатии — переключение на нужную панель
- SettingsPanel и будущие панели имеют кнопку "Назад" для возврата

## Implementation Steps

1. Выделить NavigationService из MainForm
2. Создать Feature Services интерфейсы
3. Рефакторить DiscoveryPanelView → использовать IDiscoveryPanelService
4. Создать ChatPanelView из ChatForm
5. Создать SettingsPanelView из SettingsForm
6. Настроить NavigationService с регистрацией панелей
7. Обновить MainSidePanelView для динамических кнопок
8. Обновить MainForm для использования NavigationService

## Breaking Changes

- ChatForm становится ChatPanelView (UserControl)
- SettingsForm становится SettingsPanelView (UserControl)
- DiscoveryPanelView рефакторится
- MainForm упрощается — выносим логику навигации в сервис