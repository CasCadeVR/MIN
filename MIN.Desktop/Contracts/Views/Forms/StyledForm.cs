using MIN.Desktop.Contracts.Interfaces;

namespace MIN.Desktop.Contracts.Views.Forms
{
    public partial class StyledForm : BaseForm, IStyled
    {
        public StyledForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyStylings();
        }

        void IStyled.ApplyStylings()
        {
            ApplyStylings();
        }

        protected virtual void ApplyStylings()
        {
            this.BackColor = ColorScheme.FormBackground;
        }
    }
}
