namespace MIN.Services.Connection.Contracts.Interfaces.Cryptographing
{
    /// <summary>
    /// Помощник для хранения ключей
    /// </summary>
    public interface IKeyProvider
    {
        /// <summary>
        /// Получить или создать AES ключ
        /// </summary>
        byte[] GetOrCreateAesKey();
    }
}
