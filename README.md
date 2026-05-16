# MIN - Local Messenger

![Logo](https://raw.githubusercontent.com/CasCadeVR/MIN/main/Desktop/MIN.Desktop/Resources/logo.png)

> **Безопасный локальный мессенджер с end-to-end шифрованием для локальной сети**

[![Stars](https://img.shields.io/github/stars/CasCadeVR/MIN?style=flat-square&logo=github)](https://github.com/CasCadeVR/MIN/stargazers)
[![Forks](https://img.shields.io/github/forks/CasCadeVR/MIN?style=flat-square&logo=github)](https://github.com/CasCadeVR/MIN/network/members)
[![License](https://img.shields.io/github/license/CasCadeVR/MIN?style=flat-square)](https://github.com/CasCadeVR/MIN/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-Latest-blue?style=flat-square&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

---

## 📑 Содержание

- [🎯 Возможности](#-возможности)
- [🏗️ Архитектура](#-архитектура)
- [🛠️ Технологический стек](#-технологический-стек)
- [🚀 Установка и запуск](#-установка-и-запуск)
- [📁 Структура проекта](#-структура-проекта)
- [🔐 Безопасность](#-безопасность)
- [📸 Интерфейс](#-интерфейс)
- [🤝 Авторы](#-авторы)

---

## 🎯 Возможности

| Возможность | Описание |
|-------------|----------|
| 🔒 **End-to-End шифрование** | Все сообщения шифруются с использованием криптографии |
| 📁 **Передача файлов** | Отправка и получение фотографий и файлов |
| 🌐 **Локальное обнаружение** | Автоматическое обнаружение комнат через Named Pipes |
| 💬 **Текстовые сообщения** | Отправка и получение сообщений в реальном времени |
| 👥 **Комнаты** | Создание и присоединение к чат-комнатам |
| 🖥️ **Desktop UI** | Интуитивный интерфейс на WinForms |

> [!IMPORTANT]
> MIN работает **без сервера** в вашей локальной сети. Не требуется подключение к интернету!

---

## 🏗️ Архитектура

```mermaid
flowchart TB
    %%{init: {'flowchart': {'nodeSpacing': 12}}}%%
    classDef uiLayer fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:#1e40af
    classDef infraLayer fill:#e0e7ff,stroke:#4f46e5,stroke-width:2px,color:#3730a3
    classDef coreLayer fill:#ede9fe,stroke:#7c3aed,stroke-width:2px,color:#5b21b6
    classDef component fill:#ffffff,stroke:#c7d2fe,stroke-width:1.5px,color:#312e81
    classDef highlight fill:#c7d2fe,stroke:#4f46e5,stroke-width:3px,color:#312e81
    
    subgraph UI["🖥️ UI Layer (WinForms)"]
        direction TB
        A["💬 ChatPanel<br><small>Сообщения и комнаты</small>"]
        B["🔍 DiscoveryPanel<br><small>Обнаружение комнат</small>"]
        C["📁 FileTransferPanel<br><small>Обмен файлами</small>"]
    end
    
    subgraph INF["🏗️ Infrastructure Layer"]
        direction TB
        D["💬 Chat Services<br><small>Сообщения и статусы</small>"]
        E["🔍 Discovery Services<br><small>Named Pipes Discovery</small>"]
        F["📁 FileTransfer Services<br><small>Потоковая передача</small>"]
    end
    
    subgraph CORE["⚙️ Core Layer"]
        direction TB
        G["🔐 Cryptography<br><small>E2E шифрование</small>"]
        H["📨 Messaging<br><small>Система сообщений</small>"]
        I["⚡ Events<br><small>Event Bus</small>"]
        J["🔄 Handlers<br><small>Диспетчер</small>"]
        K["🔌 Transport<br><small>Named Pipes</small>"]
        L["📋 Serialization<br><small>JSON</small>"]
    end
    
    class A highlight;
    class B,C,D,E,F,G,H,I,J,K,L component
    class UI uiLayer;
    class INF infraLayer;
    class CORE coreLayer
    
    UI --> INF
    INF --> CORE
```

### Модули

#### Core (Ядро)
| Модуль | Назначение |
|--------|------------|
| `MIN.Core.Cryptography` | Криптографические операции |
| `MIN.Core.Messaging` | Система сообщений |
| `MIN.Core.Events` | Event Bus |
| `MIN.Core.Handlers` | Обработчики с dispatcher |
| `MIN.Core.Transport` | Транспорт Named Pipes |
| `MIN.Core.Serialization` | JSON сериализация |
| `MIN.Core.Entities` | Модели данных |
| `MIN.Core.Stores` | Хранилища |
| `MIN.Core.Streaming` | Потоки данных |

#### Infrastructure (Бизнес-логика)
| Модуль | Назначение |
|--------|------------|
| `MIN.Chat` | Чаты и сообщения |
| `MIN.Discovery` | Обнаружение комнат |
| `MIN.FileTransfer` | Передача файлов |

#### Desktop (UI)
| Модуль | Назначение |
|--------|------------|
| `MIN.Desktop` | WinForms приложение |

---

## 🛠️ Технологический стек

| Категория | Технология | Описание |
|-----------|------------|----------|
| 🔷 Язык | C# | Современный объектно-ориентированный язык |
| ⚙️ Платформа | .NET 8.0 | Кроссплатформенный фреймворк |
| 🖼️ UI | WinForms | Настольный интерфейс Windows |
| 📦 DI | Microsoft.Extensions.DependencyInjection | Внедрение зависимостей |
| 🔐 Защита | System.Security.Cryptography.ProtectedData | Windows DPAPI |
| 🔌 Транспорт | Named Pipes | Межпроцессное взаимодействие |
| 🧪 Тестирование | xUnit, FluentAssertions, Moq | Модульные тесты |
| 📋 Стиль | .editorconfig | Единый код-стайл |

---

## 🚀 Установка и запуск

### Требования

> [!TIP]
> Убедитесь, что у вас установлен .NET SDK 8.0 или выше.

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) или выше
- Windows 10/11
- Локальная сеть (для обнаружения комнат)

### Быстрый старт

```bash
# 1. Клонирование репозитория
git clone https://github.com/CasCadeVR/MIN.git
cd MIN

# 2. Восстановление зависимостей
dotnet restore

# 3. Сборка проекта
dotnet build

# 4. Запуск приложения
dotnet run --project Desktop/MIN.Desktop/MIN.Desktop.csproj
```

### Запуск тестов

```bash
dotnet test
```

---

## 📁 Структура проекта

```mermaid
flowchart TB
    classDef coreFolder fill:#ede9fe,stroke:#7c3aed,color:#5b21b6
    classDef infraFolder fill:#e0e7ff,stroke:#4f46e5,color:#3730a3
    classDef deskFolder fill:#dbeafe,stroke:#3b82f6,color:#1e40af
    classDef folder fill:#f5f3ff,stroke:#c7c2ea,color:#312e81
    classDef file fill:#fff,stroke:#c7d2fe,color:#4338ca
    
    subgraph MIN["📂 MIN"]
        subgraph CORE["⚙️ Core/"]
            G["Cryptography/"]
            H["Messaging/"]
            I["Events/"]
            J["Handlers/"]
            K["Transport/"]
            L["Serialization/"]
        end
        subgraph INF["🏗️ Infrastructure/"]
            M["Chat/"]
            N["Discovery/"]
            O["FileTransfer/"]
        end
        subgraph DESK["🖥️ Desktop/"]
            P["MIN.Desktop/"]
            Q["Views/"]
            R["Components/"]
            S["Resources/"]
        end
        T["Helpers/"]
        U["Common/"]
    end
    
    class CORE coreFolder
    class INF infraFolder
    class DESK deskFolder
    class G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U file
```

> [!NOTE]
> Конфигурационные файлы (`Directory.Build.props`, `Directory.Packages.props`) управляют зависимостями централизованно.

---

## 🔐 Безопасность

> [!IMPORTANT]
> MIN использует **end-to-end шифрование** — ваши сообщения защищены от перехвата.

### Схема шифрования

```mermaid
flowchart LR
    classDef sender fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:#1e40af
    classDef encrypted fill:#e0e7ff,stroke:#4f46e5,stroke-width:3px,color:#3730a3
    classDef recipient fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:#1e40af
    
    A["👤 Отправитель"] -->|"🔐 Шифрование"| B["🔐 Зашифрованное сообщение"]
    B -->|"🌐 Локальная сеть"| C["👤 Получатель"]
    
    class A sender;
    class B encrypted;
    class C recipient
```

### Технологии защиты

| Технология | Назначение |
|------------|------------|
| 🔑 **Асимметричное (RSA)** | Обмен ключами между участниками |
| 🛡️ **Симметричное (AES)** | Шифрование содержимого сообщений |
| 💾 **Windows DPAPI** | Защищённое хранение ключей |

---

## 📸 Интерфейс

MIN предоставляет современный интерфейс для:

| Функция | Описание |
|---------|----------|
| 🏠 **Комнаты** | Создание и управление чат-комнатами |
| 💬 **Чат** | Общение в реальном времени |
| 📁 **Файлы** | Передача с отображением прогресса |
| 👥 **Участники** | Статусы онлайн/оффлайн |

> [!NOTE]
> 📸 Скриншоты будут добавлены после первого релиза.

---

## 🤝 Авторы

| Автор | Роль | Вклад |
|-------|------|-------|
| 👨‍💻 [**CasCadeVR**](https://github.com/CasCadeVR) | Основатель | Основной разработчик, создатель архитектуры |
| 💡 [**Karo4a**](https://github.com/Karo4a) | Вдохновитель | Идеи и вдохновение |

---

> [!TIP]
> **Сделано с ❤️ командой CasCade**

*MIN — Локальный мессенджер для вашей сети*

---

[![Stars](https://img.shields.io/github/stars/CasCadeVR/MIN?style=social)](https://github.com/CasCadeVR/MIN)
[![Forks](https://img.shields.io/github/forks/CasCadeVR/MIN?style=social)](https://github.com/CasCadeVR/MIN)

[Исходный код](https://github.com/CasCadeVR/MIN) • 
[Сообщить об ошибке](https://github.com/CasCadeVR/MIN/issues) • 
[Лицензия MIT](LICENSE)