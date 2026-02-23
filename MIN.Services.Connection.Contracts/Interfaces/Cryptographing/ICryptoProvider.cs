namespace MIN.Services.Connection.Contracts.Interfaces.Cryptographing
{
    /// <summary>
    /// Помощник в шифровании
    /// </summary>
    public interface ICryptoProvider
    {
        /// <summary>
        /// Имя комнаты для которой ведётся шифровка
        /// </summary>
        string RoomName { get; set; }

        /// <summary>
        /// Закодировать сообщение
        /// </summary>
        byte[] EncryptMessage(byte[] data);

        /// <summary>
        /// Раскодировать сообщение
        /// </summary>
        byte[] DecryptMessage(byte[] encryptedData);
    }
}
