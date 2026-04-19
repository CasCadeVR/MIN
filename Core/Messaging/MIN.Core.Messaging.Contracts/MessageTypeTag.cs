namespace MIN.Core.Messaging.Contracts;

/// <summary>
/// Определяет типы сообщений для маршрутизации.
/// Значения структурированы по диапазонам для обеспечения расширяемости.
/// </summary>
/// <remarks>
/// Диапазоны значений:
/// 0-31   - Системные сообщения
/// 32-63  - Сообщения обнаружения
/// 64-95  - Сообщения чата
/// 96-127 - Сообщения управления комнатой
/// 128-159 - Сообщения для передачи файлов
/// 160-191 - Сообщения для событий/интеграции
/// 192-255 - Зарезервировано для будущих категорий
/// </remarks>
public enum MessageTypeTag : byte
{
    // ===== Системные сообщения (0-31) =====

    /// <summary>
    /// Сердцебиение для поддержания соединения.
    /// </summary>
    Heartbeat = 0,

    /// <summary>
    /// Приветственное сообщение при установке соединения (содержит публичный ключ).
    /// </summary>
    Handshake = 1,

    /// <summary>
    /// Подтверждение рукопожатия.
    /// </summary>
    HandshakeAck = 2,

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    Error = 3,

    /// <summary>
    /// Закрытие соединения.
    /// </summary>
    Disconnect = 4,

    /// <summary>
    /// Проверка доступности (ping).
    /// </summary>
    Ping = 5,

    /// <summary>
    /// Ответ на ping (pong).
    /// </summary>
    Pong = 6,

    // ===== Сообщения обнаружения (32-63) =====

    /// <summary>
    /// Запрос на обнаружение активных комнат в локальной сети.
    /// </summary>
    DiscoveryRequest = 32,

    /// <summary>
    /// Ответ на запрос обнаружения, содержащий информацию о комнате.
    /// </summary>
    DiscoveryResponse = 33,

    /// <summary>
    /// Широковещательное уведомление о появлении новой комнаты.
    /// </summary>
    RoomAnnouncement = 34,

    /// <summary>
    /// Запрос на получение полной информации о комнате.
    /// </summary>
    RoomInfoRequest = 35,

    /// <summary>
    /// Подробная информация о комнате.
    /// </summary>
    RoomInfoResponse = 36,

    // ===== Сообщения чата (64-95) =====

    /// <summary>
    /// Обычное текстовое сообщение чата.
    /// </summary>
    ChatTextMessage = 64,

    /// <summary>
    /// Индикатор набора текста.
    /// </summary>
    TypingIndicator = 65,

    /// <summary>
    /// Запрос истории сообщений.
    /// </summary>
    HistoryRequest = 66,

    /// <summary>
    /// Ответ с историей сообщений.
    /// </summary>
    HistoryResponse = 67,

    /// <summary>
    /// Удаление сообщения.
    /// </summary>
    MessageDelete = 68,

    /// <summary>
    /// Редактирование сообщения.
    /// </summary>
    MessageEdit = 69,

    /// <summary>
    /// Реакция на сообщение (лайк, эмодзи).
    /// </summary>
    MessageReaction = 70,

    /// <summary>
    /// Системные сообщения (участник зашёл, загрузка)
    /// </summary>
    SystemMessage = 71,

    // ===== Сообщения управления комнатой (96-127) =====

    /// <summary>
    /// Запрос на создание комнаты.
    /// </summary>
    RoomCreateRequest = 96,

    /// <summary>
    /// Ответ на создание комнаты.
    /// </summary>
    RoomCreateResponse = 97,

    /// <summary>
    /// Запрос на присоединение к комнате.
    /// </summary>
    RoomJoinRequest = 98,

    /// <summary>
    /// Ответ на запрос присоединения.
    /// </summary>
    RoomJoinResponse = 99,

    /// <summary>
    /// Уведомление о присоединении нового участника.
    /// </summary>
    ParticipantJoined = 100,

    /// <summary>
    /// Подтверждение о присоединении нового участника.
    /// </summary>
    ParticipantAccepted = 101,

    /// <summary>
    /// Уведомление о выходе участника.
    /// </summary>
    ParticipantLeft = 102,

    /// <summary>
    /// Обновление информации об участнике.
    /// </summary>
    ParticipantUpdated = 103,

    /// <summary>
    /// Сообщение о миграции хоста.
    /// </summary>
    HostMigration = 104,

    /// <summary>
    /// Изменение состояния комнаты.
    /// </summary>
    RoomStateChanged = 105,

    // ===== Сообщения для передачи файлов (128-159) =====

    /// <summary>
    /// Метаданные файла (имя, размер, тип).
    /// </summary>
    FileMetadata = 128,

    /// <summary>
    /// Запрос на передачу файла.
    /// </summary>
    FileTransferRequest = 129,

    /// <summary>
    /// Ответ на запрос передачи файла.
    /// </summary>
    FileTransferResponse = 130,

    /// <summary>
    /// пакет (фрагмент) файла.
    /// </summary>
    FileChunk = 131,

    /// <summary>
    /// Подтверждение получения пакета.
    /// </summary>
    FileChunkAck = 132,

    /// <summary>
    /// Завершение передачи файла.
    /// </summary>
    FileTransferComplete = 133,

    /// <summary>
    /// Отмена передачи файла.
    /// </summary>
    FileTransferCancel = 134,

    // ===== Сообщения для событий/интеграции (160-191) =====

    /// <summary>
    /// Кастомное событие для интеграции с другими приложениями.
    /// </summary>
    CustomEvent = 160,

    /// <summary>
    /// Подписка на события.
    /// </summary>
    EventSubscribe = 161,

    /// <summary>
    /// Отписка от событий.
    /// </summary>
    EventUnsubscribe = 162,

    /// <summary>
    /// Публикация события.
    /// </summary>
    EventPublish = 163,

    // ===== Зарезервировано (192-255) =====
    // Свободные диапазоны для будущих категорий
}
