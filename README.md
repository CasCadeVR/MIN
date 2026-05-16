# MIN - Local Messenger

<div style="max-width: 1100px; margin: 0 auto;">
  <style scoped>
    .hero-card { background: linear-gradient(135deg, #f5f3ff 0%, #ede9fe 100%); border: 2px solid #4f46e5; border-radius: 12px; padding: 32px; margin-bottom: 32px; }
    .hero-content { display: flex; align-items: center; gap: 32px; }
    .hero-logo { flex-shrink: 0; }
    .hero-logo img { width: 120px; height: 120px; border-radius: 12px; }
    .hero-text { flex: 1; }
    .hero-title { font-size: 36px; font-weight: 900; color: #312e81; margin: 0 0 12px 0; letter-spacing: -0.02em; }
    .hero-subtitle { font-size: 18px; color: #4338ca; margin: 0 0 20px 0; font-weight: 400; }
    .hero-desc { font-size: 14px; color: #3730a3; line-height: 1.7; margin: 0; }
    .hero-badges { display: flex; gap: 8px; flex-wrap: wrap; margin-top: 16px; }
    .hero-bar { height: 4px; background: linear-gradient(90deg, #4f46e5 0%, #7c3aed 50%, #a855f7 100%); border-radius: 2px; margin-bottom: 24px; }
    .toc-card { background: rgba(255, 255, 255, 0.85); border: 1px solid #c7d2fe; border-radius: 8px; padding: 20px; margin-bottom: 32px; }
    .toc-title { font-size: 14px; font-weight: bold; color: #312e81; margin-bottom: 12px; text-transform: uppercase; letter-spacing: 1px; }
    .toc-list { display: flex; flex-wrap: wrap; gap: 8px; list-style: none; padding: 0; margin: 0; }
    .toc-item { font-size: 12px; padding: 6px 12px; background: #f5f3ff; border: 1px solid #c7c2ea; border-radius: 16px; }
    .toc-item a { color: #4338ca; text-decoration: none; }
    .toc-item a:hover { color: #4f46e5; }
    .section-card { background: #f5f3ff; border: 1px solid #c7c2ea; border-radius: 8px; padding: 24px; margin-bottom: 24px; position: relative; }
    .section-title { font-size: 20px; font-weight: bold; color: #312e81; margin-bottom: 16px; display: flex; align-items: center; gap: 10px; }
    .accent-bar { position: absolute; top: 0; left: 0; width: 100%; height: 4px; background: linear-gradient(90deg, #4f46e5, #7c3aed); border-radius: 8px 8px 0 0; }
    .feat-bento { display: grid; grid-template-columns: 2fr 1fr 1fr; grid-template-rows: 1fr 1fr; gap: 12px; }
    .feat-card { border-radius: 8px; padding: 20px; background: rgba(255, 255, 255, 0.9); border: 1px solid #c7d2fe; }
    .feat-card.hero { grid-row: span 2; background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%); border: 2px solid #4f46e5; display: flex; flex-direction: column; justify-content: center; }
    .feat-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(67, 56, 202, 0.1); }
    .feat-icon { font-size: 28px; margin-bottom: 8px; }
    .feat-title { font-size: 14px; font-weight: bold; color: #312e81; margin-bottom: 6px; }
    .feat-desc { font-size: 11px; color: #4338ca; line-height: 1.4; }
    .arch-layer { margin: 12px 0; padding: 16px; border-radius: 8px; }
    .arch-layer-title { font-size: 13px; font-weight: bold; margin-bottom: 12px; text-align: center; }
    .arch-grid { display: grid; gap: 10px; }
    .arch-grid-3 { grid-template-columns: repeat(3, 1fr); }
    .arch-grid-6 { grid-template-columns: repeat(6, 1fr); }
    .arch-box { border-radius: 6px; padding: 10px; text-align: center; font-size: 11px; font-weight: 600; color: #312e81; background: rgba(255, 255, 255, 0.85); border: 1px solid #c7d2fe; }
    .arch-box.highlight { background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%); border: 2px solid #4f46e5; }
    .arch-box.tech { font-size: 10px; color: #4338ca; background: rgba(238, 242, 255, 0.8); }
    .arch-layer.ui { background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%); border: 2px solid #3b82f6; }
    .arch-layer.ui .arch-layer-title { color: #1e40af; }
    .arch-layer.infra { background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%); border: 2px solid #4f46e5; }
    .arch-layer.infra .arch-layer-title { color: #3730a3; }
    .arch-layer.core { background: linear-gradient(135deg, #ede9fe 0%, #ddd6fe 100%); border: 2px solid #7c3aed; }
    .arch-layer.core .arch-layer-title { color: #5b21b6; }
    .tech-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; }
    .tech-card { border-radius: 8px; padding: 16px; background: rgba(255, 255, 255, 0.85); border: 1px solid #c7d2fe; text-align: center; }
    .tech-icon { font-size: 28px; margin-bottom: 8px; }
    .tech-name { font-size: 13px; font-weight: bold; color: #312e81; margin-bottom: 4px; }
    .tech-desc { font-size: 10px; color: #4338ca; }
    .install-timeline { position: relative; padding-left: 40px; }
    .install-timeline::before { content: ''; position: absolute; left: 15px; top: 0; bottom: 0; width: 2px; background: linear-gradient(180deg, #4f46e5, #7c3aed); }
    .install-step { position: relative; margin-bottom: 20px; padding: 16px; background: rgba(255, 255, 255, 0.85); border-radius: 8px; border: 1px solid #c7d2fe; }
    .install-step::before { content: ''; position: absolute; left: -33px; top: 18px; width: 12px; height: 12px; border-radius: 50%; background: #4f46e5; border: 3px solid #f5f3ff; }
    .install-step-num { font-size: 10px; font-weight: bold; color: #4f46e5; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 4px; }
    .install-step-title { font-size: 14px; font-weight: bold; color: #312e81; margin-bottom: 8px; }
    .install-step-code { background: #1e1e2e; color: #cdd6f4; padding: 12px; border-radius: 6px; font-family: 'Consolas', monospace; font-size: 11px; line-height: 1.6; }
    .install-step-code .cmd { color: #a6e3a1; }
    .install-step-code .cmt { color: #6c7086; }
    .install-req { margin-top: 20px; padding: 16px; background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%); border: 2px solid #3b82f6; border-radius: 8px; }
    .install-req-title { font-size: 12px; font-weight: bold; color: #1e40af; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px; }
    .install-req-item { font-size: 11px; color: #1e40af; margin: 4px 0; }
    .struct-bento { display: grid; grid-template-columns: 1.5fr 1fr; gap: 16px; }
    .struct-section { border-radius: 8px; padding: 16px; }
    .struct-section.core { background: linear-gradient(135deg, #ede9fe 0%, #ddd6fe 100%); border: 2px solid #7c3aed; }
    .struct-section.side { display: grid; gap: 12px; }
    .struct-section.infra { background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%); border: 2px solid #4f46e5; }
    .struct-section.desktop { background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%); border: 2px solid #3b82f6; }
    .struct-title { font-size: 13px; font-weight: bold; margin-bottom: 12px; text-align: center; padding-bottom: 8px; border-bottom: 2px solid; }
    .struct-section.core .struct-title { color: #5b21b6; border-color: #7c3aed; }
    .struct-section.infra .struct-title { color: #3730a3; border-color: #4f46e5; }
    .struct-section.desktop .struct-title { color: #1e40af; border-color: #3b82f6; }
    .struct-items { display: flex; flex-wrap: wrap; gap: 6px; }
    .struct-item { font-size: 10px; padding: 4px 10px; border-radius: 12px; background: rgba(255, 255, 255, 0.9); border: 1px solid rgba(0,0,0,0.1); color: #312e81; }
    .sec-diagram { display: flex; align-items: center; justify-content: center; gap: 16px; margin: 20px 0; flex-wrap: wrap; }
    .sec-node { border-radius: 10px; padding: 16px 20px; text-align: center; font-weight: 600; min-width: 100px; }
    .sec-user { background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%); border: 2px solid #3b82f6; color: #1e40af; }
    .sec-encrypt { background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%); border: 2px solid #4f46e5; color: #3730a3; }
    .sec-network { background: linear-gradient(135deg, #ede9fe 0%, #ddd6fe 100%); border: 2px solid #7c3aed; color: #5b21b6; }
    .sec-recipient { background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%); border: 2px solid #3b82f6; color: #1e40af; }
    .sec-arrow { font-size: 20px; color: #4f46e5; }
    .sec-features { display: grid; grid-template-columns: repeat(3, 1fr); gap: 12px; margin-top: 20px; }
    .sec-feature { border-radius: 8px; padding: 14px; background: rgba(255, 255, 255, 0.85); border: 1px solid #c7d2fe; text-align: center; }
    .sec-feature-icon { font-size: 24px; margin-bottom: 8px; }
    .sec-feature-title { font-size: 12px; font-weight: bold; color: #312e81; margin-bottom: 4px; }
    .sec-feature-desc { font-size: 10px; color: #4338ca; }
    .ui-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; }
    .ui-card { border-radius: 8px; padding: 16px; background: rgba(255, 255, 255, 0.9); border: 1px solid #c7d2fe; text-align: center; }
    .ui-icon { font-size: 32px; margin-bottom: 8px; }
    .ui-title { font-size: 12px; font-weight: bold; color: #312e81; margin-bottom: 4px; }
    .ui-desc { font-size: 10px; color: #4338ca; }
    .ui-placeholder { grid-column: span 4; border-radius: 8px; padding: 40px; background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%); border: 2px dashed #94a3b8; text-align: center; color: #64748b; font-size: 14px; }
    .authors-bento { display: grid; grid-template-columns: 1.5fr 1fr; gap: 20px; }
    .author-card { border-radius: 10px; padding: 24px; text-align: center; }
    .author-main { background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%); border: 2px solid #4f46e5; }
    .author-contrib { background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%); border: 2px solid #94a3b8; }
    .author-avatar { font-size: 48px; margin-bottom: 12px; }
    .author-name { font-size: 18px; font-weight: bold; color: #312e81; margin-bottom: 6px; }
    .author-role { font-size: 12px; color: #4338ca; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 1px; }
    .author-desc { font-size: 12px; color: #5b21b6; line-height: 1.5; }
    .author-links { margin-top: 16px; display: flex; justify-content: center; gap: 16px; }
    .author-links a { font-size: 12px; color: #4f46e5; text-decoration: none; }
    .footer-card { background: linear-gradient(135deg, #312e81 0%, #1e1b4b 100%); border-radius: 12px; padding: 32px; text-align: center; color: white; }
    .footer-logo { font-size: 32px; margin-bottom: 12px; }
    .footer-text { font-size: 16px; margin-bottom: 8px; }
    .footer-sub { font-size: 12px; opacity: 0.8; }
    .footer-links { margin-top: 16px; display: flex; justify-content: center; gap: 24px; }
    .footer-links a { font-size: 12px; color: #c7d2fe; text-decoration: none; }
    .highlight-quote { border-left: 4px solid #4f46e5; padding-left: 16px; margin: 16px 0; font-size: 14px; color: #312e81; font-style: italic; }
  </style>

  <div class="hero-card">
    <div class="accent-bar"></div>
    <div class="hero-content">
      <div class="hero-logo">
        <img src="https://raw.githubusercontent.com/CasCadeVR/MIN/main/Desktop/MIN.Desktop/Resources/logo.png" alt="MIN Logo">
      </div>
      <div class="hero-text">
        <h1 class="hero-title">MIN</h1>
        <p class="hero-subtitle">Локальный мессенджер нового поколения</p>
        <p class="hero-desc">Безопасный мессенджер с end-to-end шифрованием для локальной сети. Общайтесь с коллегами и друзьями без серверов и зависимости от интернета.</p>
        <p class="highlight-quote">Ваши сообщения — только ваши. Никто не перехватит.</p>
        <div class="hero-badges">
          <a href="https://github.com/CasCadeVR/MIN/stargazers"><img src="https://img.shields.io/github/stars/CasCadeVR/MIN?style=flat-square&logo=github" alt="Stars"></a>
          <a href="https://github.com/CasCadeVR/MIN/network/members"><img src="https://img.shields.io/github/forks/CasCadeVR/MIN?style=flat-square&logo=github" alt="Forks"></a>
          <a href="https://github.com/CasCadeVR/MIN/blob/main/LICENSE"><img src="https://img.shields.io/github/license/CasCadeVR/MIN?style=flat-square" alt="License"></a>
          <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet" alt=".NET"></a>
          <a href="https://docs.microsoft.com/en-us/dotnet/csharp/"><img src="https://img.shields.io/badge/C%23-Latest-blue?style=flat-square&logo=csharp" alt="C#"></a>
        </div>
      </div>
    </div>
  </div>

  <div class="toc-card">
    <div class="toc-title">Содержание</div>
    <ul class="toc-list">
      <li class="toc-item"><a href="#features">🎯 Возможности</a></li>
      <li class="toc-item"><a href="#architecture">🏗️ Архитектура</a></li>
      <li class="toc-item"><a href="#tech">🛠️ Технологии</a></li>
      <li class="toc-item"><a href="#install">🚀 Установка</a></li>
      <li class="toc-item"><a href="#structure">📁 Структура</a></li>
      <li class="toc-item"><a href="#security">🔐 Безопасность</a></li>
      <li class="toc-item"><a href="#ui">📸 Интерфейс</a></li>
      <li class="toc-item"><a href="#authors">🤝 Авторы</a></li>
    </ul>
  </div>
</div>

## 🎯 Возможности {#features}

<div class="section-card">
  <div class="accent-bar"></div>
  <div class="section-title">Что умеет MIN</div>
  <div class="feat-bento">
    <div class="feat-card hero">
      <div class="feat-icon">🔒</div>
      <div class="feat-title" style="font-size: 18px;">End-to-End шифрование</div>
      <div class="feat-desc" style="font-size: 12px;">Все сообщения шифруются с использованием криптографии. Никто не сможет перехватить вашу переписку.</div>
    </div>
    <div class="feat-card">
      <div class="feat-icon">📁</div>
      <div class="feat-title">Передача файлов</div>
      <div class="feat-desc">Отправка и получение фотографий и файлов</div>
    </div>
    <div class="feat-card">
      <div class="feat-icon">🌐</div>
      <div class="feat-title">Локальное обнаружение</div>
      <div class="feat-desc">Через Named Pipes в вашей сети</div>
    </div>
    <div class="feat-card">
      <div class="feat-icon">💬</div>
      <div class="feat-title">Текстовые сообщения</div>
      <div class="feat-desc">Общение в реальном времени</div>
    </div>
    <div class="feat-card">
      <div class="feat-icon">👥</div>
      <div class="feat-title">Комнаты</div>
      <div class="feat-desc">Создание и присоединение</div>
    </div>
    <div class="feat-card">
      <div class="feat-icon">🖥️</div>
      <div class="feat-title">Desktop UI</div>
      <div class="feat-desc">WinForms интерфейс</div>
    </div>
  </div>
</div>

## 🏗️ Архитектура {#architecture}

<div class="section-card">
  <div class="accent-bar"></div>
  <div class="section-title">Многослойная архитектура MIN</div>
  <div class="arch-layer ui">
    <div class="arch-layer-title">Слой UI — WinForms Desktop</div>
    <div class="arch-grid arch-grid-3">
      <div class="arch-box highlight">ChatPanel<br><small>Сообщения и комнаты</small></div>
      <div class="arch-box">DiscoveryPanel<br><small>Обнаружение комнат</small></div>
      <div class="arch-box">FileTransferPanel<br><small>Обмен файлами</small></div>
    </div>
  </div>
  <div class="arch-layer infra">
    <div class="arch-layer-title">Слой Infrastructure — Бизнес-логика</div>
    <div class="arch-grid arch-grid-3">
      <div class="arch-box">Chat Services<br><small>Сообщения и статусы</small></div>
      <div class="arch-box">Discovery Services<br><small>Обнаружение Named Pipes</small></div>
      <div class="arch-box">FileTransfer Services<br><small>Потоковая передача</small></div>
    </div>
  </div>
  <div class="arch-layer core">
    <div class="arch-layer-title">Слой Core — Ядро системы</div>
    <div class="arch-grid arch-grid-6">
      <div class="arch-box tech">Cryptography<br><small>E2E шифрование</small></div>
      <div class="arch-box tech">Messaging<br><small>Система сообщений</small></div>
      <div class="arch-box tech">Events<br><small>Event Bus</small></div>
      <div class="arch-box tech">Handlers<br><small>Диспетчер</small></div>
      <div class="arch-box tech">Transport<br><small>Named Pipes</small></div>
      <div class="arch-box tech">Serialization<br><small>JSON</small></div>
    </div>
  </div>
</div>

## 🛠️ Технологический стек {#tech}

<div class="section-card">
  <div class="accent-bar"></div>
  <div class="section-title">Технологии проекта</div>
  <div class="tech-grid">
    <div class="tech-card"><div class="tech-icon">🔷</div><div class="tech-name">C#</div><div class="tech-desc">Язык разработки</div></div>
    <div class="tech-card"><div class="tech-icon">⚙️</div><div class="tech-name">.NET 8.0</div><div class="tech-desc">Платформа</div></div>
    <div class="tech-card"><div class="tech-icon">🖼️</div><div class="tech-name">WinForms</div><div class="tech-desc">Пользовательский интерфейс</div></div>
    <div class="tech-card"><div class="tech-icon">📦</div><div class="tech-name">Microsoft.Extensions.DI</div><div class="tech-desc">Dependency Injection</div></div>
    <div class="tech-card"><div class="tech-icon">🔐</div><div class="tech-name">DPAPI</div><div class="tech-desc">Защита данных Windows</div></div>
    <div class="tech-card"><div class="tech-icon">🔌</div><div class="tech-name">Named Pipes</div><div class="tech-desc">Локальный транспорт</div></div>
    <div class="tech-card"><div class="tech-icon">🧪</div><div class="tech-name">xUnit</div><div class="tech-desc">Тестирование</div></div>
    <div class="tech-card"><div class="tech-icon">📋</div><div class="tech-name">.editorconfig</div><div class="tech-desc">Стандарты кода</div></div>
  </div>
</div>

## 🚀 Установка и запуск {#install}

<div class="section-card">
  <div class="accent-bar"></div>
  <div class="section-title">Быстрый старт</div>
  <div class="install-timeline">
    <div class="install-step">
      <div class="install-step-num">Шаг 1</div>
      <div class="install-step-title">Клонируйте репозиторий</div>
      <div class="install-step-code"><span class="cmd">git clone</span> https://github.com/CasCadeVR/MIN.git<br><span class="cmt">cd MIN</span></div>
    </div>
    <div class="install-step">
      <div class="install-step-num">Шаг 2</div>
      <div class="install-step-title">Восстановите зависимости и соберите</div>
      <div class="install-step-code"><span class="cmd">dotnet restore</span><br><span class="cmd">dotnet build</span></div>
    </div>
    <div class="install-step">
      <div class="install-step-num">Шаг 3</div>
      <div class="install-step-title">Запустите приложение</div>
      <div class="install-step-code"><span class="cmd">dotnet run</span> --project Desktop/MIN.Desktop/MIN.Desktop.csproj</div>
    </div>
    <div class="install-step">
      <div class="install-step-num">Шаг 4</div>
      <div class="install-step-title">Запустите тесты</div>
      <div class="install-step-code"><span class="cmd">dotnet test</span></div>
    </div>
  </div>
  <div class="install-req">
    <div class="install-req-title">📋 Требования</div>
    <div class="install-req-item">✅ [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) или выше</div>
    <div class="install-req-item">✅ Windows 10/11</div>
    <div class="install-req-item">✅ Локальная сеть (для обнаружения комнат)</div>
  </div>
</div>

## 📁 Структура проекта {#structure}

<div class="section-card">
  <div class="accent-bar"></div>
  <div class="section-title">Модульная архитектура</div>
  <div class="struct-bento">
    <div class="struct-section core">
      <div class="struct-title">⚙️ Core (Ядро)</div>
      <div class="struct-items">
        <span class="struct-item">Cryptography</span>
        <span class="struct-item">Messaging</span>
        <span class="struct-item">Events</span>
        <span class="struct-item">Handlers</span>
        <span class="struct-item">Transport</span>
        <span class="struct-item">Serialization</span>
        <span class="struct-item">Entities</span>
        <span class="struct-item">Stores</span>
        <span class="struct-item">Streaming</span>
        <span class="struct-item">Headers</span>
        <span class="struct-item">DI</span>
      </div>
    </div>
    <div class="struct-section side">
      <div class="struct-section infra">
        <div class="struct-title">🏗️ Infrastructure</div>
        <div class="struct-items">
          <span class="struct-item">Chat</span>
          <span class="struct-item">Discovery</span>
          <span class="struct-item">FileTransfer</span>
        </div>
      </div>
      <div class="struct-section desktop">
        <div class="struct-title">🖥️ Desktop</div>
        <div class="struct-items">
          <span class="struct-item">MIN.Desktop</span>
          <span class="struct-item">Views</span>
          <span class="struct-item">Components</span>
          <span class="struct-item">Resources</span>
        </div>
      </div>
    </div>
  </div>
  <p style="text-align: center; margin-top: 16px; font-size: 11px; color: #4338ca;">Также: Helpers, Common, MIN.sln, Directory.Build.props, Directory.Packages.props</p>
</div>

## 🔐 Безопасность {#security}

<div class="section-card">
  <div class="accent-bar"></div>
  <div class="section-title">End-to-End шифрование</div>
  <div class="sec-diagram">
    <div class="sec-node sec-user">👤 Отправитель<br><small>Шифрование</small></div>
    <div class="sec-arrow">🔒→</div>
    <div class="sec-node sec-encrypt">🔐 Зашифровано<br><small>сообщение</small></div>
    <div class="sec-arrow">→</div>
    <div class="sec-node sec-network">🌐 Локальная<br><small>сеть</small></div>
    <div class="sec-arrow">→</div>
    <div class="sec-node sec-recipient">👤 Получатель<br><small>Расшифровка</small></div>
  </div>
  <div class="sec-features">
    <div class="sec-feature">
      <div class="sec-feature-icon">🔑</div>
      <div class="sec-feature-title">Асимметричное</div>
      <div class="sec-feature-desc">Обмен ключами RSA</div>
    </div>
    <div class="sec-feature">
      <div class="sec-feature-icon">🛡️</div>
      <div class="sec-feature-title">Симметричное</div>
      <div class="sec-feature-desc">Защита содержимого</div>
    </div>
    <div class="sec-feature">
      <div class="sec-feature-icon">💾</div>
      <div class="sec-feature-title">Windows DPAPI</div>
      <div class="sec-feature-desc">Безопасное хранение</div>
    </div>
  </div>
</div>

## 📸 Интерфейс {#ui}

<div class="section-card">
  <div class="accent-bar"></div>
  <div class="section-title">Возможности интерфейса</div>
  <div class="ui-placeholder">📸 Скриншоты будут добавлены после первого релиза</div>
  <div class="ui-grid" style="margin-top: 16px;">
    <div class="ui-card"><div class="ui-icon">🏠</div><div class="ui-title">Комнаты</div><div class="ui-desc">Создание и управление</div></div>
    <div class="ui-card"><div class="ui-icon">💬</div><div class="ui-title">Чат</div><div class="ui-desc">Общение в реальном времени</div></div>
    <div class="ui-card"><div class="ui-icon">📁</div><div class="ui-title">Файлы</div><div class="ui-desc">Передача с прогрессом</div></div>
    <div class="ui-card"><div class="ui-icon">👥</div><div class="ui-title">Участники</div><div class="ui-desc">Статусы онлайн/оффлайн</div></div>
  </div>
</div>

## 🤝 Авторы {#authors}

<div class="section-card">
  <div class="accent-bar"></div>
  <div class="section-title">Команда проекта</div>
  <div class="authors-bento">
    <div class="author-card author-main">
      <div class="author-avatar">👨‍💻</div>
      <div class="author-name">CasCadeVR</div>
      <div class="author-role">Основатель проекта</div>
      <div class="author-desc">Основной разработчик, создатель архитектуры и ключевых компонентов MIN</div>
    </div>
    <div class="author-card author-contrib">
      <div class="author-avatar">💡</div>
      <div class="author-name">Karo4a</div>
      <div class="author-role">Идеи и вдохновение</div>
      <div class="author-desc">Вдохновлял, предлагал идеи и немного помогал с реализацией</div>
    </div>
  </div>
  <div class="author-links" style="margin-top: 20px;">
    <a href="https://github.com/CasCadeVR">🏢 CasCade</a>
    <a href="https://github.com/Karo4a">👤 Профиль</a>
  </div>
</div>

<div class="footer-card" style="margin-top: 32px;">
  <div class="footer-logo">💬</div>
  <div class="footer-text">Сделано с ❤️ командой CasCade</div>
  <div class="footer-sub">MIN — Локальный мессенджер для вашей сети</div>
  <div class="footer-links">
    <a href="https://github.com/CasCadeVR/MIN">📂 Исходный код</a>
    <a href="https://github.com/CasCadeVR/MIN/issues">🐛 Сообщить об ошибке</a>
    <a href="LICENSE">📄 Лицензия MIT</a>
  </div>
</div>

</div>