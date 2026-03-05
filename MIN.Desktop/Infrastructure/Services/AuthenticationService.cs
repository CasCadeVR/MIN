using MIN.Services.Contracts.Models;

namespace MIN.Desktop.Infrastructure.Services
{
    /// <summary>
    /// Сервис для работы с пользователем приложения
    /// </summary>
    public class AppUserProvider
    {
        private static readonly Lazy<AppUserProvider> _instance =
            new Lazy<AppUserProvider>(() => new AppUserProvider());

        /// <summary>
        /// Подгрузка экземпляра по появлению пользователя
        /// </summary>
        public static AppUserProvider Instance => _instance.Value;

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public Participant CurrentUser { get; private set; }

        private AppUserProvider() { }

        /// <summary>
        /// Авторизировать пользователя
        /// </summary>
        public void InitializeUser(Participant user)
        {
            CurrentUser = user;
        }
    }
}