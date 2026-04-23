using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Contracts.Views.PanelViews;
using MIN.DI;
using MIN.Helpers.Contracts.Models;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Desktop.Views.Panels.SidePanelViews;

/// <summary>
/// Боковая панель настроек
/// </summary>
public partial class SettingsSidePanelView : StyledPanelView
{
    private readonly IMinFeatureCollection featureCollection;

    /// <summary>
    /// Текущие настройки
    /// </summary>
    public Settings Settings { get; set; } = null!;

    /// <inheritdoc />
    public override PanelType PanelType => PanelType.Side;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainSidePanelView"/>
    /// </summary>
    public SettingsSidePanelView(IMinFeatureCollection featureCollection)
    {
        InitializeComponent();
        this.featureCollection = featureCollection;
        FillControls();
        EnableOutOfRadioButtons();
    }

    private void FillControls()
    {
        labelVersion.Text += featureCollection.Version.ToString();
        defaultName.Text = Settings.DefaultParticipantName;
        roomSearchTime.Value = Settings.DiscoveryTimeout;
        preferredSearch.Checked = Settings.SearchMethod == SearchMethod.Preferred;
        classRoomSearch.Checked = Settings.SearchMethod == SearchMethod.ClassRoom;
        SetPCNames(Settings.PreferredPCNames.ToList());
    }

    private void SetPCNames(List<string> pcNames)
    {
        var allowAdd = preferredPcNameList.AllowUserToAddRows;
        preferredPcNameList.AllowUserToAddRows = false;
        preferredPcNameList.Rows.Clear();

        if (pcNames != null)
        {
            foreach (var pcName in pcNames)
            {
                var cleanName = pcName.Trim();
                if (!string.IsNullOrEmpty(cleanName))
                {
                    preferredPcNameList.Rows.Add(cleanName);
                }
            }
        }

        preferredPcNameList.AllowUserToAddRows = allowAdd;
    }

    /// <inheritdoc />
    protected override void ApplyStylings()
    {
        splitContainer.Panel1.BackColor = ColorScheme.PrimaryAccent;
        splitContainer.Panel2.BackColor = ColorScheme.MainPanelBackground;
        Title.ForeColor = ColorScheme.TextOnAccent;
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        Settings.DefaultParticipantName = defaultName.Text;
        Settings.DiscoveryTimeout = Convert.ToInt32(roomSearchTime.Value);
        Settings.SearchMethod = preferredSearch.Checked ? SearchMethod.Preferred : SearchMethod.ClassRoom;
        Settings.PreferredPCNames = GetPCNames();

        featureCollection.Helper.SettingsProvider.SaveSettings(Settings);
    }

    private List<string> GetPCNames()
    {
        var pcNames = new List<string>();

        foreach (DataGridViewRow row in preferredPcNameList.Rows)
        {
            if (row.IsNewRow)
            {
                continue;
            }

            var cellValue = row.Cells[0].Value;

            if (cellValue != null)
            {
                var pcName = cellValue.ToString()!.Trim();
                if (!string.IsNullOrEmpty(pcName))
                {
                    pcNames.Add(pcName);
                }
            }
        }

        return pcNames;
    }

    private void EnableOutOfRadioButtons()
    {
        classRoomDescription.Enabled = classRoomSearch.Checked;
        pcNameDescription.Enabled = preferredSearch.Checked;
        preferredPcNameDescription.Enabled = preferredSearch.Checked;
        preferredPcNameList.Enabled = preferredSearch.Checked;
    }

    private void preferredSearch_CheckedChanged(object sender, EventArgs e)
    {
        EnableOutOfRadioButtons();
    }

    private void classRoomSearch_CheckedChanged(object sender, EventArgs e)
    {
        EnableOutOfRadioButtons();
    }

    private void preferredPcNameList_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
        if (e.ColumnIndex == 0)
        {
            var newValue = e.FormattedValue!.ToString()!.Trim();
            if (string.IsNullOrEmpty(newValue))
            {
                return;
            }

            foreach (DataGridViewRow row in preferredPcNameList.Rows)
            {
                if (row.IsNewRow || row.Index == e.RowIndex)
                {
                    continue;
                }

                var existing = row.Cells[0].Value.ToString()!.Trim();
                if (string.Equals(existing, newValue, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Такое имя компьютера уже существует.");
                    e.Cancel = true;
                    return;
                }
            }
        }
    }

    private void logButton_Click(object sender, EventArgs e)
    {
        new LogForm(featureCollection.Helper.Logger).Show();
    }
}
