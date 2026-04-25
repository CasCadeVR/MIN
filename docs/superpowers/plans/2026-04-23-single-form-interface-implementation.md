# Single-Form Interface Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Одна форма с навигируемыми панелями (single-form interface)

**Architecture:** 
- `IMinFeatureCollection` - единая точка доступа ко всем сервисам (уже создан в DI/MIN.DI)
- Панели создаются через DI с конструктором `IMinFeatureCollection`
- Runtime параметры через `IPanelInitializer<TParams>`
- Навигация через generic `INavigationService` (отказались от NavigationItem)

**Tech Stack:** WinForms, .NET, Microsoft.Extensions.DependencyInjection

---

## Key Changes: No More NavigationItem

**Отказываемся от NavigationItem** в пользу generic подхода:

**Было (сложно):**
```csharp
nav.NavigateTo(new NavigationItem {
    PanelType = PanelType.Main,
    ViewInstance = new DiscoveryPanelView(...),
    Parent = currentItem
});
```

**Стало (просто):**
```csharp
nav.NavigateTo<DiscoveryPanelView>();  // без параметров
nav.NavigateTo<ChatPanelView, (Guid, Guid)>((roomId, connectionId));  // с параметрами
```

**Плюсы:**
- Не нужно создавать NavigationItem каждый раз
- Тип панели = generic тип - сразу понятно куда навигируем
- Compile-time проверка типов
- Меньше файлов

---

## File Structure

```
Desktop/MIN.Desktop/
├── Contracts/
│   ├── Views/
│   │   └── PanelViews/
│   │       ├── BasePanelView.cs       (существует)
│   │       └── IPanelInitializer.cs  (NEW - generic interface)
│   └── Interfaces/
│       └── INavigationService.cs    (существует)
├── Views/
│   ├── Forms/
│   │   └── MainForm.cs
│   └── Panels/
│       ├── PanelViews/
│       │   ├── ChatPanelView.cs    (NEW - из ChatForm)
│       │   └── DiscoveryPanelView.cs (изменить)
│       └── SidePanelViews/
│           ├── MainSidePanelView.cs
│           └── SettingsPanelView.cs (NEW - из SettingsForm)
└── Infrastructure/
    └── Services/
        └── NavigationService.cs (NEW)
```

Feature Collections (уже созданы):
```
DI/
└── MIN.DI/
    └── IMinFeatureCollection.cs  (существует)
---

## NavigationService + MainForm Connection

### Проблема:
MainForm создаётся в Program.cs вручную (не через DI), но ему нужен INavigationService, который требует Panel.

### Решение - Panel через property:

```csharp
public class NavigationService : INavigationService
{
    public Panel MainPanel { get; set; }  // устанавливается после создания
    public Panel SidePanel { get; set; }
    private readonly IServiceProvider _provider;

    public NavigationService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void NavigateTo<TPanel>() where TPanel : BasePanelView
    {
        var panel = _provider.GetRequiredService<TPanel>();
        MainPanel.Controls.Clear();
        panel.Dock = DockStyle.Fill;
        MainPanel.Controls.Add(panel);
    }
}
```

В Program.cs:
```csharp
var nav = new NavigationService(provider);
var mainForm = new MainForm(nav);
nav.MainPanel = mainForm.MainPanelContainer;  // после InitializeComponent
nav.SidePanel = mainForm.SidePanelContainer;
```

---
Панели должны получить CancellationTokenSource времени жизни приложения без его передачи в конструктор.

### Решение:

Создать ICtsProvider в DI:

```csharp
public interface ICtsProvider
{
    CancellationTokenSource AppCts { get; }
    CancellationToken Token { get; }
}

// Регистрируется один раз в MinModule
services.AddSingleton<ICtsProvider, CtsProvider>();

// Панели получают через конструктор
public DiscoveryPanelView(IMinFeatureCollection features, ICtsProvider ctsProvider)
{
    var token = ctsProvider.Token;  // можно передавать в async операции
}
```

---

## RecentRoomCard + ChatPanelView Management

### Задача:
При входе в комнату (создание/присоединение) добавлять RecentRoomCard в MainSidePanelView. При нажатии на карточку - переход на ChatPanelView.

### Проблема:
- ChatPanelView создаётся каждый раз заново = пересоздание UI
- Или берётся из ServiceProvider = всегда новый экземпляр

### Решение:

**MainSidePanelView хранит Dictionary<Guid, ChatPanelView> activeChats:**
1. При входе в комнату - создаётся ChatPanelView, добавляется в Dictionary
2. В flowLayoutPanel добавляется RecentRoomCard с информацией о комнате
3. При нажатии на карточку - берём существующий ChatPanelView из Dictionary, вызываем NavigateTo
4. При закрытии чата - удаляем из Dictionary

**Подход:**
- DiscoveryPanelView и ChatPanelView НЕ берутся из ServiceProvider напрямую
- Вместо этого NavigationService содержит IServiceProvider для создания панелей через DI
- Каждая панель создаётся один раз и хранится в MainSidePanelView

### Как работает:
1. DiscoveryPanelView при входе в комнату уведомляет MainSidePanelView (через событие или интерфейс)
2. MainSidePanelView создаёт ChatPanelView через INavigationService.CreatePanel (который использует IServiceProvider)
3. ChatPanelView сохраняется в Dictionary<Guid, ChatPanelView>
4. В recentFlowLayoutPanel добавляется RecentRoomCard
5. При нажатии - NavigateTo на существующий ChatPanelView

### Интерфейс для связи:
```csharp
public interface IChatRoomManager
{
    void RegisterChat(Guid roomId, ChatPanelView panel);
    void UnregisterChat(Guid roomId);
    ChatPanelView? GetChat(Guid roomId);
    event Action<Guid, ChatPanelView> OnChatRegistered;
}
```

---

---

## Approach: Constructor DI + IPanelInitializer

### Почему так:

1. **IMinFeatureCollection** - содержит все сервисы (Chat, Core, Discovery, Helper)
2. **Каждая панель** получает `IMinFeatureCollection` через конструктор из DI
3. **Runtime параметры** (roomId, connectionId) передаются через `Initialize()`
4. **INavigationService** управляет показом панелей

### Как это работает:

```csharp
// MainForm
public MainForm(IMinFeatureCollection features, INavigationService nav)
{
    // Показываем DiscoveryPanel - он создаётся через DI автоматически
    nav.NavigateTo<DiscoveryPanelView>();
}

// DiscoveryPanelView -只需要 IMinFeatureCollection
public class DiscoveryPanelView : BasePanelView
{
    public DiscoveryPanelView(IMinFeatureCollection features)
    {
        // использует features.Discovery, features.Core, features.Helper
    }
}

// ChatPanelView - IMinFeatureCollection + runtime параметры
public class ChatPanelView : BasePanelView, IPanelInitializer<(Guid roomId, Guid connectionId)>
{
    private readonly IMinFeatureCollection _features;
    
    public ChatPanelView(IMinFeatureCollection features)
    {
        _features = features;
    }

    public void Initialize((Guid roomId, Guid connectionId) parameters)
    {
        // инициализация с roomId, connectionId
    }
}
```

---

## Task 1: IPanelInitializer + INavigationService

**Files:**
- Create: `Desktop/MIN.Desktop/Contracts/Views/PanelViews/IPanelInitializer.cs`
- Create: `Desktop/MIN.Desktop/Contracts/Views/PanelViews/IChatRoomManager.cs`
- Create: `Desktop/MIN.Desktop/Infrastructure/Services/NavigationService.cs`

- [ ] **Step 1: Создать IPanelInitializer**

```csharp
namespace MIN.Desktop.Contracts.Views.PanelViews;

public interface IPanelInitializer<TParams>
{
    void Initialize(TParams parameters);
}

public interface IPanel
{
    void OnNavigatedTo();
}
```

- [ ] **Step 2: Создать IChatRoomManager**

```csharp
namespace MIN.Desktop.Contracts.Interfaces;

public interface IChatRoomManager
{
    void RegisterChat(Guid roomId, ChatPanelView panel);
    void UnregisterChat(Guid roomId);
    ChatPanelView? GetChat(Guid roomId);
    IReadOnlyDictionary<Guid, ChatPanelView> ActiveChats { get; }
    event Action<Guid, ChatPanelView> OnChatRegistered;
    event Action<Guid> OnChatUnregistered;
}
```

- [ ] **Step 3: Создать NavigationService**

```csharp
public class NavigationService : INavigationService
{
    public Panel MainPanel { get; set; }
    public Panel SidePanel { get; set; }
    
    private readonly IServiceProvider _provider;
    private readonly IChatRoomManager _chatRoomManager;

    public NavigationService(IServiceProvider provider, IChatRoomManager chatRoomManager)
    {
        _provider = provider;
        _chatRoomManager = chatRoomManager;
    }

    public void NavigateTo<TPanel>() where TPanel : BasePanelView
    {
        var panel = _provider.GetRequiredService<TPanel>();
        ShowInMainPanel(panel);
    }

    public void NavigateTo<TPanel, TParams>(TParams param) where TPanel : BasePanelView
    {
        var panel = _provider.GetRequiredService<TPanel>();
        if (panel is IPanelInitializer<TParams> init)
            init.Initialize(param);
        ShowInMainPanel(panel);
    }

    public void NavigateToExistingChat(Guid roomId)
    {
        var chat = _chatRoomManager.GetChat(roomId);
        if (chat != null)
            ShowInMainPanel(chat);
    }

    private void ShowInMainPanel(BasePanelView panel)
    {
        MainPanel.Controls.Clear();
        panel.Dock = DockStyle.Fill;
        MainPanel.Controls.Add(panel);
        if (panel is IPanel p)
            p.OnNavigatedTo();
    }
}
```

Обновить INavigationService:

```csharp
public interface INavigationService
{
    void NavigateTo<TPanel>() where TPanel : BasePanelView;
    void NavigateTo<TPanel, TParams>(TParams param) where TPanel : BasePanelView;
    void NavigateToExistingChat(Guid roomId);
}
```

- [ ] **Step 4: Commit**

---

## Task 2: DiscoveryPanelView с IMinFeatureCollection

**Files:**
- Modify: `Desktop/MIN.Desktop/Views/Panels/PanelViews/DiscoveryPanelView.cs`

- [ ] **Step 1: Обновить конструктор**

```csharp
public partial class DiscoveryPanelView : BasePanelView
{
    private readonly IMinFeatureCollection _features;

    public DiscoveryPanelView(IMinFeatureCollection features)
    {
        _features = features;
        InitializeComponent();
    }
    
    // Использовать _features.Discovery, _features.Core, _features.Helper
}
```

- [ ] **Step 2: При входе в комнату - регистрация в IChatRoomManager**

При подключении к комнате получаем IChatRoomManager и INavigationService из IMinFeatureCollection.Core, создаём ChatPanelView через NavigationService.CreateChatPanel и регистрируем:

```csharp
// В DiscoveryPanelView при успешном подключении к комнате
var chatRoomManager = _features.Core.GetService<IChatRoomManager>();
var nav = _features.Core.GetService<INavigationService>();

// Создаём и инициализируем ChatPanelView
var chat = nav.CreateChatPanel(roomId, connectionId);
chatRoomManager.RegisterChat(roomId, chat);

// Показываем
nav.NavigateToExistingChat(roomId);
```

- [ ] **Step 3: Commit**

---

## Task 3: ChatPanelView с IMinFeatureCollection + Initialize()

**Files:**
- Create: `Desktop/MIN.Desktop/Views/Panels/PanelViews/ChatPanelView.cs`

- [ ] **Step 1: Создать ChatPanelView**

```csharp
public partial class ChatPanelView : BasePanelView, 
    IPanelInitializer<(Guid roomId, Guid connectionId)>
{
    private readonly IMinFeatureCollection _features;
    private Guid _roomId;
    private Guid _connectionId;

    public ChatPanelView(IMinFeatureCollection features)
    {
        _features = features;
        InitializeComponent();
    }

    public void Initialize((Guid roomId, Guid connectionId) parameters)
    {
        _roomId = parameters.roomId;
        _connectionId = parameters.connectionId;
        // Подключение к комнате через _features
    }
}
```

- [ ] **Step 2: Commit**

---

## Task 4: MainSidePanelView с IChatRoomManager

**Files:**
- Modify: `Desktop/MIN.Desktop/Views/Panels/SidePanelViews/MainSidePanelView.cs`

- [ ] **Step 1: Обновить MainSidePanelView**

MainSidePanelView реализует IChatRoomManager и хранит Dictionary<Guid, ChatPanelView>:
- При RegisterChat - добавляет в Dictionary и создаёт RecentRoomCard
- При нажатии на карточку - вызывает NavigateToExistingChat

```csharp
public partial class MainSidePanelView : BasePanelView, IChatRoomManager
{
    private readonly Dictionary<Guid, ChatPanelView> _activeChats = new();
    private readonly INavigationService _navService;

    public MainSidePanelView(INavigationService navService)
    {
        _navService = navService;
        InitializeComponent();
    }

    public void RegisterChat(Guid roomId, ChatPanelView panel)
    {
        _activeChats[roomId] = panel;
        var card = new RecentRoomCard(panel.RoomInfo);
        card.OnClick = () => _navService.NavigateToExistingChat(roomId);
        _recentRoomsPanel.Controls.Add(card);
    }

    public void UnregisterChat(Guid roomId)
    {
        if (_activeChats.Remove(roomId))
        {
            // Удалить карточку из UI
        }
    }
}
```

- [ ] **Step 2: Commit**

---

## Task 5: DI Registration + CtsProvider

**Files:**
- Create: `DI/MIN.DI/CtsProvider.cs`
- Create: `DI/MIN.DI/ChatRoomManager.cs`
- Modify: `DI/MIN.DI/MinModule.cs`

- [ ] **Step 1: Создать CtsProvider**

```csharp
public interface ICtsProvider
{
    CancellationTokenSource AppCts { get; }
    CancellationToken Token { get; }
}

public class CtsProvider : ICtsProvider
{
    public CancellationTokenSource AppCts { get; } = new();
    public CancellationToken Token => AppCts.Token;
}
```

- [ ] **Step 2: Создать ChatRoomManager**

```csharp
public class ChatRoomManager : IChatRoomManager
{
    private readonly Dictionary<Guid, ChatPanelView> _chats = new();
    
    public event Action<Guid, ChatPanelView> OnChatRegistered = delegate { };
    public event Action<Guid> OnChatUnregistered = delegate { };

    public void RegisterChat(Guid roomId, ChatPanelView panel)
    {
        _chats[roomId] = panel;
        OnChatRegistered(roomId, panel);
    }

    public void UnregisterChat(Guid roomId)
    {
        if (_chats.Remove(roomId))
            OnChatUnregistered(roomId);
    }

    public ChatPanelView? GetChat(Guid roomId) => _chats.GetValueOrDefault(roomId);
    public IReadOnlyDictionary<Guid, ChatPanelView> ActiveChats => _chats;
}
```

- [ ] **Step 3: Обновить MinModule**

```csharp
public class MinModule : Module
{
    protected override void Load(IServiceCollection services)
    {
        services.RegisterModule<HelpersModule>();
        services.RegisterModule<CoreModule>();
        services.RegisterModule<ChatModule>();
        services.RegisterModule<DiscoveryModule>();

        // IMinFeatureCollection
        services.AddSingleton<IMinFeatureCollection, MinFeatureCollection>();
        
        // CTS Provider
        services.AddSingleton<ICtsProvider, CtsProvider>();

        // Chat Room Manager
        services.AddSingleton<IChatRoomManager, ChatRoomManager>();

        // Panels
        services.AddTransient<DiscoveryPanelView>();
        services.AddTransient<ChatPanelView>();
        services.AddTransient<MainSidePanelView>();
        services.AddTransient<SettingsPanelView>();

        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();
    }
}
```

- [ ] **Step 4: Commit**

---

## Task 6: MainForm Integration + Program.cs

**Files:**
- Modify: `Desktop/MIN.Desktop/Views/Forms/MainForm.cs`
- Modify: `Desktop/MIN.Desktop/Program.cs`

- [ ] **Step 1: Обновить MainForm**

MainForm получает INavigationService и IChatRoomManager через конструктор. После InitializeComponent() устанавливает Panel в NavigationService:

```csharp
public partial class MainForm : StyledForm
{
    private readonly INavigationService _navigationService;
    private readonly IChatRoomManager _chatRoomManager;

    public MainForm(INavigationService navigationService, IChatRoomManager chatRoomManager)
    {
        _navigationService = navigationService;
        _chatRoomManager = chatRoomManager;
        InitializeComponent();
        
        // Навигация настраивается после InitializeComponent
        _navigationService.MainPanel = mainPanel;
        _navigationService.SidePanel = sidePanel;
        
        // Показать side и discovery при старте
        _navigationService.NavigateTo<MainSidePanelView>();
        _navigationService.NavigateTo<DiscoveryPanelView>();
    }
}
```

- [ ] **Step 2: Обновить Program.cs**

MainForm создаётся вручную (не через DI), поэтому NavigationService и ChatRoomManager создаются вручную:

```csharp
var provider = new ServiceCollection()
    .AddModules<MinModule>()
    .BuildServiceProvider();

// Создаём вручную - не через DI
var chatRoomManager = new ChatRoomManager();
var navService = new NavigationService(provider, chatRoomManager);

var mainForm = new MainForm(navService, chatRoomManager);
navService.MainPanel = mainForm.MainPanelContainer;
navService.SidePanel = mainForm.SidePanelContainer;

Application.Run(mainForm);
```

- [ ] **Step 3: Commit**

---

## Summary

| Task | Description | Files |
|------|-------------|-------|
| 1 | IPanelInitializer + INavigationService | 3 new |
| 2 | DiscoveryPanelView | 1 modified |
| 3 | ChatPanelView | 1 new |
| 4 | MainSidePanelView + IChatRoomManager | 1 modified |
| 5 | DI Registration + CtsProvider + ChatRoomManager | 3 files |
| 6 | MainForm + Program.cs | 2 modified |

---

## Verification

```bash
cd Desktop/MIN.Desktop && dotnet build
```

## Task 1: NavigationService + IPanelInitializer

**Approach:** Выносим навигацию в сервис, добавляем generic interface для runtime параметров.

**Files:**
- Create: `Desktop/MIN.Desktop/Contracts/Views/PanelViews/IPanelInitializer.cs`
- Create: `Desktop/MIN.Desktop/Infrastructure/Services/NavigationService.cs`
- Modify: `Desktop/MIN.Desktop/Views/Forms/MainForm.cs`

```csharp
// IPanelInitializer.cs
namespace MIN.Desktop.Contracts.Views.PanelViews;

public interface IPanelInitializer<TParams>
{
    void Initialize(TParams parameters);
}
```

```csharp
// NavigationService.cs
namespace MIN.Desktop.Infrastructure.Services;

public class NavigationService : INavigationService
{
    private readonly Panel _mainPanel;
    private readonly Panel _sidePanel;
    private readonly IFeatureServiceRegistry _registry;

    public NavigationService(Panel mainPanel, Panel sidePanel, IFeatureServiceRegistry registry)
    {
        _mainPanel = mainPanel;
        _sidePanel = sidePanel;
        _registry = registry;
    }

    public void NavigateTo(NavigationItem item)
    {
        var container = item.PanelType == PanelType.Main ? _mainPanel : _sidePanel;
        container.Controls.Clear();

        if (item.ViewInstance != null)
        {
            item.ViewInstance.Dock = DockStyle.Fill;
            container.Controls.Add(item.ViewInstance);
            item.ViewInstance.OnNavigation(item);
        }
    }
}
```

- [ ] **Step 1: Создать IPanelInitializer**

- [ ] **Step 2: Создать NavigationService**

- [ ] **Step 3: Обновить MainForm**

- [ ] **Step 4: Commit**

```csharp
// NavigationService.cs
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Contracts.Models.Enums;
using MIN.Desktop.Contracts.Views.PanelViews;

namespace MIN.Desktop.Infrastructure.Services;

public class NavigationService : INavigationService
{
    private readonly Panel mainPanel;
    private readonly Panel sidePanel;
    private readonly Dictionary<PanelType, BasePanelView> activeViews = new();
    private BasePanelView? currentMainView;
    private BasePanelView? currentSideView;

    public NavigationService(Panel mainPanel, Panel sidePanel)
    {
        this.mainPanel = mainPanel;
        this.sidePanel = sidePanel;
    }

    public void NavigateTo(NavigationItem item)
    {
        if (currentMainView != null)
        {
            currentMainView.RequestNavigate -= OnNavigateRequested;
        }

        var container = item.PanelType == PanelType.Main ? mainPanel : sidePanel;

        if (item.ViewInstance != null)
        {
            currentMainView = item.ViewInstance;
            currentMainView.RequestNavigate += OnNavigateRequested;
            currentMainView.OnNavigation(item);
            currentMainView.Dock = DockStyle.Fill;
            container.Controls.Clear();
            container.Controls.Add(currentMainView);
        }
    }

    private void OnNavigateRequested(NavigationItem item) => NavigateTo(item);
}
```

- [ ] **Step 3: Обновить MainForm**

Упростить MainForm - использовать NavigationService вместо ручной логики:

```csharp
// MainForm.cs - сокращенно
public partial class MainForm : StyledForm, INavigationService
{
    private readonly NavigationService navigationService;
    
    public MainForm(/* сервисы */)
    {
        InitializeComponent();
        navigationService = new NavigationService(mainPanel, sidePanel);
        InitializeDefaultViews();
    }

    private void InitializeDefaultViews()
    {
        navigationService.NavigateTo(new NavigationItem
        {
            PanelType = PanelType.Side,
            ViewInstance = new MainSidePanelView()
        });
        
        navigationService.NavigateTo(new NavigationItem
        {
            PanelType = PanelType.Main,
            ViewInstance = new DiscoveryPanelView(/* IDiscoveryPanelService */)
        });
    }

    public void NavigateTo(NavigationItem item) => navigationService.NavigateTo(item);
}
```

- [ ] **Step 4: Commit**

```bash
git add Contracts/ Infrastructure/ Views/Forms/MainForm.cs
git commit -m "feat: add NavigationService and feature service interfaces"
```

---

## Task 2: ChatPanelView - Constructor DI + Initialize()

**Approach:** Классический DI с runtime параметрами через Initialize()

- Панель регистрируется в DI контейнере
- Сервисы инжектится через конструктор
- Runtime параметры (room, endpoint) передаются через Initialize()

**Files:**
- Modify: `Desktop/MIN.Desktop/Views/Panels/PanelViews/ChatForm.cs`
- Create: `Desktop/MIN.Desktop/Contracts/Views/PanelViews/IPanelInitializer.cs`

```csharp
// IPanelInitializer.cs - generic interface for runtime params
namespace MIN.Desktop.Contracts.Views.PanelViews;

public interface IPanelInitializer<TParams>
{
    void Initialize(TParams parameters);
}
```

```csharp
// ChatPanelView - из ChatForm с изменениями
public partial class ChatPanelView : BasePanelView, IPanelInitializer<(Room room, IEndpoint endpoint)>
{
    private readonly IChatPanelService _service;
    private Room? _room;
    private IEndpoint? _endpoint;

    // Конструктор - DI внедряет сервисы
    public ChatPanelView(IChatPanelService service)
    {
        _service = service;
        InitializeComponent();
    }

    // Runtime параметры через Initialize
    public void Initialize((Room room, IEndpoint endpoint) parameters)
    {
        _room = parameters.room;
        _endpoint = parameters.endpoint;
        
        _service.CurrentRoom = parameters.room;
        _service.Endpoint = parameters.endpoint;
        
        // инициализация UI - комната, название, участники
    }

    // Остальной код использует _service.ChatService вместо прямых сервисов
}
```

- [ ] **Step 1: Создать IPanelInitializer**

- [ ] **Step 2: Обновить ChatForm -> ChatPanelView**

- [ ] **Step 3: Commit**

- [ ] **Step 2: Обновить Designer файл**

Нужно изменить base class в Designer файле:
- Найти `this.BaseStylingForm = ` заменить на `this.BaseView = `

- [ ] **Step 3: Commit**

```bash
git add Views/Panels/PanelViews/ChatPanelView.cs
git commit -m "feat: create ChatPanelView from ChatForm"
```

---

## Task 3: SettingsPanelView - Constructor DI

**Approach:** Простая панель - только конструктор DI, без runtime параметров.

**Files:**
- Create: `Desktop/MIN.Desktop/Views/Panels/SidePanelViews/SettingsPanelView.cs`

```csharp
public partial class SettingsPanelView : BasePanelView
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly Version _version;
    
    public Settings Settings { get; private set; }

    // Конструктор - DI внедряет сервисы
    public SettingsPanelView(ISettingsProvider settingsProvider, Version version)
    {
        _settingsProvider = settingsProvider;
        _version = version;
        Settings = settingsProvider.GetSettings();
        InitializeComponent();
    }

    // Кнопка назад
    private void OnBackClicked(object sender, EventArgs e)
    {
        if (CurrentNavigationItem?.Parent != null)
        {
            RequestNavigate?.Invoke(CurrentNavigationItem.Parent);
        }
    }
}
```

- [ ] **Step 1: Создать SettingsPanelView**

- [ ] **Step 2: Commit**

---

## Task 4: MainSidePanelView with Dynamic Buttons + DI

**Approach:** Конструктор DI, динамические кнопки меню.

**Files:**
- Modify: `Desktop/MIN.Desktop/Views/Panels/SidePanelViews/MainSidePanelView.cs`

```csharp
public partial class MainSidePanelView : BasePanelView
{
    private readonly INavigationService _navigationService;
    private readonly IFeatureServiceRegistry _featureRegistry;

    // Конструктор - DI
    public MainSidePanelView(INavigationService navigationService, IFeatureServiceRegistry featureRegistry)
    {
        _navigationService = navigationService;
        _featureRegistry = featureRegistry;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // существующий код + добавить кнопки меню
        
        var discoveryButton = new Button { Text = "Обнаружение" };
        discoveryButton.Click += (s, e) => NavigateToDiscovery();
        
        var settingsButton = new Button { Text = "Настройки" };
        settingsButton.Click += (s, e) => NavigateToSettings();
        
        menuFlowPanel.Controls.Add(discoveryButton);
        menuFlowPanel.Controls.Add(settingsButton);
    }

    private void NavigateToDiscovery()
    {
        // Получаем сервис из Registry и создаём панель
        var service = _featureRegistry.GetDiscoveryPanelService();
        var panel = new DiscoveryPanelView(service);
        panel.CurrentNavigationItem = new NavigationItem
        {
            PanelType = PanelType.Main,
            Parent = CurrentNavigationItem
        };
        
        RequestNavigate?.Invoke(new NavigationItem
        {
            PanelType = PanelType.Main,
            ViewInstance = panel
        });
    }

    private void NavigateToSettings()
    {
        var panel = new SettingsPanelView(
            _featureRegistry.GetSettingsProvider(),
            _featureRegistry.GetVersion());
            
        RequestNavigate?.Invoke(new NavigationItem
        {
            PanelType = PanelType.Side,
            ViewInstance = panel
        });
    }
}
```

- [ ] **Step 1: Обновить MainSidePanelView**

- [ ] **Step 2: Commit**

---

## Task 5: DI Registration + FeatureServiceRegistry Update

**Files:**
- Modify: `DI/MIN.DI/MinModule.cs`
- Modify: `DI/MIN.FeatureServices.Contracts/FeatureServiceRegistry.cs`

Добавить регистрации в MinModule:

```csharp
public class MinModule : Module
{
    protected override void Load(IServiceCollection services)
    {
        services.RegisterModule<HelpersModule>();
        services.RegisterModule<CoreModule>();
        services.RegisterModule<ChatModule>();
        services.RegisterModule<DiscoveryModule>();

        // Feature Services (уже зарегистрированы)
        services.AddSingleton<IDiscoveryPanelService, DiscoveryPanelService>();
        services.AddTransient<IChatPanelService, ChatPanelService>();

        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();

        // Panels - регистрируем чтобы DI мог создать
        services.AddTransient<DiscoveryPanelView>();
        services.AddTransient<ChatPanelView>();
        services.AddTransient<SettingsPanelView>();
        services.AddTransient<MainSidePanelView>();
    }
}
```

**FeatureServiceRegistry** должен предоставлять методы для получения сервисов:

```csharp
public interface IFeatureServiceRegistry
{
    // ... существующие методы
    
    T GetService<T>() where T : class;
    T ResolvePanel<T>() where T : BasePanelView;
}

public class FeatureServiceRegistry : IFeatureServiceRegistry
{
    private readonly IServiceProvider _provider;

    public T GetService<T>() where T : class
    {
        return _provider.GetRequiredService<T>();
    }

    public T ResolvePanel<T>() where T : BasePanelView
    {
        return _provider.GetRequiredService<T>();
    }
}
```

- [ ] **Step 1: Обновить MinModule**

- [ ] **Step 2: Обновить FeatureServiceRegistry**

- [ ] **Step 3: Commit**

---

## Task 6: Integration

**Files:**
- Modify: несколько файлов для финальной интеграции

- [ ] **Step 1: Убедиться что DiscoveryPanelView использует IDiscoveryPanelService**

Обновить конструктор DiscoveryPanelView:

```csharp
public DiscoveryPanelView(IDiscoveryPanelService service)
{
    this.service = service;
    // ...
}
```

- [ ] **Step 2: Проверить билд**

```bash
cd Desktop/MIN.Desktop && dotnet build
```

- [ ] **Step 3: Commit**

```bash
git add . && git commit -m "feat: integrate single-form navigation"
```

---

## Task 7: Verify and Clean Up

- [ ] **Step 1: Проверить что всё компилируется**
- [ ] **Step 2: Удалить старые Forms (ChatForm, SettingsForm) если не используются**
- [ ] **Step 3: Commit cleanup**

```bash
git add . && git commit -m "chore: remove unused form files"
```

---

## Summary

| Task | Description | Files |
|------|-------------|-------|
| 1 | NavigationService | 3 new, 1 modified |
| 2 | ChatPanelView | 1 new (from ChatForm) |
| 3 | SettingsPanelView | 1 new (from SettingsForm) |
| 4 | MainSidePanelView | 1 modified |
| 5 | DI Registration | 1 modified |
| 6 | Integration | multiple |
| 7 | Cleanup | - |

---

## Verification Commands

```bash
# Build project
cd Desktop/MIN.Desktop && dotnet build

# Run (if debug)
cd Desktop/MIN.Desktop && dotnet run
```