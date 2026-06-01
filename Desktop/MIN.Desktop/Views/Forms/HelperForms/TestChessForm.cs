using MIN.Desktop.Contracts.Views.Forms;

namespace MIN.Desktop.Views.Forms.HelperForms
{
    /// <summary>
    /// Тест - удалить
    /// </summary>
    public partial class TestChessForm : StyledForm
    {
        /// <summary>
        /// Тест - удалить
        /// </summary>
        public Action? OnClose { get; set; }

        /// <summary>
        /// Тест - удалить
        /// </summary>
        public TestChessForm(string title)
        {
            InitializeComponent();
            heading1Label1.Text += " " + title;
        }

        private void TestChessForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnClose?.Invoke();
        }
    }
}
