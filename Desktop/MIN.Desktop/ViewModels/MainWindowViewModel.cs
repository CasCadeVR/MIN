namespace MIN.Desktop.ViewModels
{
    /// <summary>
    /// Модель главного окна
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Начальное слово
        /// </summary>
        public string Greeting { get; } = "Welcome to Avalonia!";
    }
}
