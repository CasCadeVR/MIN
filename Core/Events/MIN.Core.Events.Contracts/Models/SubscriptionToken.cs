namespace MIN.Core.Events.Contracts.Models
{
    /// <summary>
    /// Токен подписки
    /// </summary>
    public class SubscriptionToken : IDisposable
    {
        private readonly Action unsubscribe;
        private bool disposed;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SubscriptionToken"/>
        /// </summary>
        public SubscriptionToken(Action unsubscribe)
        {
            this.unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            unsubscribe();
        }
    }
}
