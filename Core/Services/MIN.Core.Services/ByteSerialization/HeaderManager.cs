using MIN.Core.Services.Contracts.Interfaces.ByteSerialization;

namespace MIN.Core.Services.ByteSerialization
{
    /// <summary>
    /// <inheritdoc cref="IHeaderManager"/>
    /// </summary>
    public class HeaderManager : IHeaderManager
    {
        bool IHeaderManager.IsEncrypted(byte[] data) => data.Length > 0 && (data[0] & 0x01) != 0;

        byte[] IHeaderManager.AddEncryptionHeader(byte[] encryptedData)
        {
            var header = new byte[] { 0x01 };
            return header.Concat(encryptedData).ToArray();
        }

        byte[] IHeaderManager.AddPlainHeader(byte[] plainData)
        {
            var header = new byte[] { 0x00 };
            return header.Concat(plainData).ToArray();
        }

        byte[] IHeaderManager.RemoveEncryptionHeader(byte[] data) => data.Skip(1).ToArray();
    }
}
