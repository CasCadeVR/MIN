using MIN.Services.Contracts.Models.Participants;

namespace MIN.Desktop.Infrastructure.Services
{
    /// <summary>
    /// Сервис для работы с пользователем приложения
    /// </summary>
    public class AppUserProvider
    {
        private readonly static Lazy<AppUserProvider> instance =
            new Lazy<AppUserProvider>(() => new AppUserProvider());

        /// <summary>
        /// Подгрузка экземпляра по появлению пользователя
        /// </summary>
        public static AppUserProvider Instance => instance.Value;

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
