using System.Linq;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Cards;
using MIN.Desktop.ViewModels.Windows;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна выбора сессии
/// </summary>
public partial class SessionChoosingViewModel : ModalViewModelBase
{
    /// <summary>
    /// Выбранная сессия
    /// </summary>
    public Session? SelectedSession { get; set; }

    /// <summary>
    /// Скаченные сессии
    /// </summary>
    [ObservableProperty]
    public partial AvaloniaList<DownloadedSessionViewModel> AvaibleSessions { get; set; } = [];

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionChoosingViewModel"/>
    /// </summary>
    public SessionChoosingViewModel(ISessionScanner sessionScanner)
    {
        var parentWindow = MainWindowViewModel.GetWindow();

        var downloadedSessions = sessionScanner.DownloadedSessions.Values;

        foreach (var session in downloadedSessions)
        {
            var card = new DownloadedSessionViewModel(session, parentWindow?.Clipboard);
            card.OnClicked += (selected) =>
            {
                foreach (var sessionVm in AvaibleSessions)
                {
                    if (sessionVm.Session.SessionId != session.SessionId)
                    {
                        sessionVm.Unselect();
                    }
                }
                SelectedSession = selected ? session : null;
                SelectSessionCommand.NotifyCanExecuteChanged();
            };
            AvaibleSessions.Add(card);
        }

        var exampleCard = AvaibleSessions.First();
        exampleCard.Select();
    }

    [RelayCommand(CanExecute = nameof(CanProceed))]
    private void SelectSession()
    {
        Close(ButtonOptions.Ok);
    }

    private bool CanProceed() => SelectedSession != null;
}
