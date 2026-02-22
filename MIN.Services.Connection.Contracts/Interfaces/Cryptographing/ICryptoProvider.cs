namespace MIN.Services.Connection.Contracts.Interfaces.Cryptographing
{
    /// <summary>
    /// Помощник в шифровании
    /// </summary>
    public interface ICryptoProvider
    {
        /// <summary>
        /// Закодировать сообщение
        /// </summary>
        Task<byte[]> EncryptMessageAsync(byte[] data);

        /// <summary>
        /// Раскодировать сообщение
        /// </summary>
        Task<byte[]> DecryptMessageAsync(byte[] encryptedData);
    }
}
