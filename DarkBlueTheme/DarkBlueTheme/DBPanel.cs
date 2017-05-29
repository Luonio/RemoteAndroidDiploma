using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    public partial class DBPanel : Panel
    {
        DBForm parentForm;

        public DBPanel(DBForm form)
        {
            parentForm = form;
            BackColor = Palette.DarkGrayBlueWorkingArea;
            SetBounds();
        }

        public void SetBounds()
        {
            Height = parentForm.ClientRectangle.Height - 10;
            Width = Convert.ToInt32(parentForm.ClientRectangle.Width - 20);
            Location = new Point(10, parentForm.ClientRectangle.Y);
        }
    }
}
