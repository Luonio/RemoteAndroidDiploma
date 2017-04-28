using System.Drawing;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    public partial class DBButton: Button
    {
        public DBButton()
        {
            InitializeComponent();
            this.FlatStyle = FlatStyle.Flat;
            this.BackColor = Color.FromArgb(48, 48, 64);
            this.ForeColor = Color.FromArgb(187, 187, 187);
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
        }
    }
}
