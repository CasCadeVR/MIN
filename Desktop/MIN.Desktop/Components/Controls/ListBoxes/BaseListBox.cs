using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.ListBoxes
{
    public class BaseListBox : ListBox
    {
        public BaseListBox()
        {
            ApplyBaseStyles();
        }

        protected virtual void ApplyBaseStyles()
        {
            Font = FontScheme.Default;
            ForeColor = ColorScheme.TextPrimary;
            BackColor = ColorScheme.InputFieldBackground;
        }
    }
}
