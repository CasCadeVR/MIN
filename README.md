# MIN - Local Messenger

![Logo](https://raw.githubusercontent.com/CasCadeVR/MIN/main/Desktop/MIN.Desktop/Resources/logo.png)

> **Secure local messenger with end-to-end encryption for local network**

[![Stars](https://img.shields.io/github/stars/CasCadeVR/MIN?style=flat-square&logo=github)](https://github.com/CasCadeVR/MIN/stargazers)
[![Forks](https://img.shields.io/github/forks/CasCadeVR/MIN?style=flat-square&logo=github)](https://github.com/CasCadeVR/MIN/network/members)
[![License](https://img.shields.io/github/license/CasCadeVR/MIN?style=flat-square)](https://github.com/CasCadeVR/MIN/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-Latest-blue?style=flat-square&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

[English](README.md) • [Русский](README.ru.md)

---

## 📑 Contents

- [🎯 Features](#-features)
- [🏗️ Architecture](#-architecture)
- [🛠️ Technology Stack](#-technology-stack)
- [🚀 Installation and Running](#-installation-and-running)
- [📁 Project Structure](#-project-structure)
- [🔐 Security](#-security)
- [📸 Interface](#-interface)
- [🤝 Authors](#-authors)

---

## 🎯 Features

| Feature | Description |
|---------|-------------|
| 🔒 **End-to-End Encryption** | All messages are encrypted using cryptography |
| 📁 **File Transfer** | Sending and receiving photos and files |
| 🌐 **Local Discovery** | Automatic room discovery via Named Pipes |
| 💬 **Text Messages** | Sending and receiving messages in real time |
| 👥 **Rooms** | Creating and joining chat rooms |
| 🖥️ **Desktop UI** | Intuitive interface using WinForms |

> [!IMPORTANT]
> MIN works **without a server** in your local network. No internet connection required!

---

## 🏗️ Architecture

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
        A["💬 ChatPanel<br>Messages and rooms"]
        B["🔍 DiscoveryPanel<br>Room discovery"]
    end
    
    subgraph INF["🏗️ Infrastructure Layer"]
        direction TB
        C["💬 Chat Services<br>Messages and statuses"]
        D["🔍 Discovery Services<br>Named Pipes Discovery"]
        E["📁 FileTransfer Services<br>Streaming transfer"]
    end
    
    subgraph CORE["⚙️ Core Layer"]
        direction TB
        F["🔐 Cryptography<br>E2E encryption"]
        G["📨 Messaging<br>Messaging system"]
        H["⚡ Events<br>Event Bus"]
        I["🔄 Handlers<br>Dispatcher"]
        J["🔌 Transport<br>Named Pipes"]
        K["📋 Serialization<br>JSON"]
    end
    
    class A highlight;
    class B,C,D,E,F,G,H,I,J,K component
    class UI uiLayer;
    class INF infraLayer;
    class CORE coreLayer
    
    UI --> INF
    INF --> CORE
```

### Modules

#### Core
| Module | Purpose |
|--------|---------|
| `MIN.Core.Cryptography` | Cryptographic operations |
| `MIN.Core.Messaging` | Messaging system |
| `MIN.Core.Events` | Event Bus |
| `MIN.Core.Handlers` | Handlers with dispatcher |
| `MIN.Core.Transport` | Named Pipes transport |
| `MIN.Core.Serialization` | JSON serialization |
| `MIN.Core.Entities` | Data models |
| `MIN.Core.Stores` | Stores |
| `MIN.Core.Streaming` | Data streams |

#### Infrastructure (Business Logic)
| Module | Purpose |
|--------|---------|
| `MIN.Chat` | Chats and messages |
| `MIN.Discovery` | Room discovery |
| `MIN.FileTransfer` | File transfer |

#### Desktop (UI)
| Module | Purpose |
|--------|---------|
| `MIN.Desktop` | WinForms application |

---

## 🛠️ Technology Stack

| Category | Technology | Description |
|----------|------------|-------------|
| 🔷 Language | C# | Modern object-oriented language |
| ⚙️ Platform | .NET 8.0 | Cross-platform framework |
| 🖼️ UI | WinForms | Windows desktop interface |
| 📦 DI | Microsoft.Extensions.DependencyInjection | Dependency injection |
| 🔐 Security | System.Security.Cryptography.ProtectedData | Windows DPAPI |
| 🔌 Transport | Named Pipes | Inter-process communication |
| 📋 Style | .editorconfig | Unified code style |

---

## 🚀 Installation and Running

### Requirements

> [!TIP]
> Make sure you have .NET SDK 8.0 or higher installed.

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- Windows 10/11
- Local network (for room discovery)

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/CasCadeVR/MIN.git
cd MIN

# 2. Restore dependencies
dotnet restore

# 3. Build the project
dotnet build

# 4. Run the application
dotnet run --project Desktop/MIN.Desktop/MIN.Desktop.csproj
```

---

## 📁 Project Structure

```mermaid
flowchart TB
    %%{init: {'flowchart': {'nodeSpacing': 20}}}%%
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
> Configuration files (`Directory.Build.props`, `Directory.Packages.props`) manage dependencies centrally.

---

## 🔐 Security

> [!IMPORTANT]
> MIN uses **end-to-end encryption** — your messages are protected from interception.

### Encryption Scheme

```mermaid
flowchart LR
    classDef sender fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:#1e40af
    classDef encrypted fill:#e0e7ff,stroke:#4f46e5,stroke-width:3px,color:#3730a3
    classDef recipient fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:#1e40af
    
    A["👤 Sender"] -->|"🔐 Encryption"| B["🔐 Encrypted Message"]
    B -->|"🌐 Local Network"| C["👤 Recipient"]
    
    class A sender;
    class B encrypted;
    class C recipient
```

### Security Technologies

| Technology | Purpose |
|------------|---------|
| 🔑 **Asymmetric (RSA)** | Key exchange between participants |
| 🛡️ **Symmetric (AES)** | Message content encryption |
| 💾 **Windows DPAPI** | Secure key storage |

---

## 📸 Interface

MIN provides a modern interface for:

| Function | Description |
|----------|-------------|
| 🏠 **Rooms** | Creating and managing chat rooms |
| 💬 **Chat** | Real-time communication |
| 📁 **Files** | Transfer with progress display |
| 👥 **Participants** | Online/offline statuses |

> [!NOTE]
> 📸 Screenshots will be added after the first release.

---

## 🤝 Authors

| Author | Role | Contribution |
|--------|------|--------------|
| 👨‍💻 [**CasCadeVR**](https://github.com/CasCadeVR) | Founder | Lead developer, architecture creator |
| 💡 [**Karo4a**](https://github.com/Karo4a) | Inspiration | Ideas and inspiration |

---

> [!TIP]
> **Made with ❤️ by CasCade team**

*MIN — Local messenger for your network*

---

[![Stars](https://img.shields.io/github/stars/CasCadeVR/MIN?style=social)](https://github.com/CasCadeVR/MIN)
[![Forks](https://img.shields.io/github/forks/CasCadeVR/MIN?style=social)](https://github.com/CasCadeVR/MIN)

[Source Code](https://github.com/CasCadeVR/MIN) • 
[Report an Issue](https://github.com/CasCadeVR/MIN/issues) • 
[MIT License](LICENSE)