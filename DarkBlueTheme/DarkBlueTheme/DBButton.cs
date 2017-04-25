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
            this.BackColor = Color.FromArgb(150, 160, 190);
        }
    }
}
