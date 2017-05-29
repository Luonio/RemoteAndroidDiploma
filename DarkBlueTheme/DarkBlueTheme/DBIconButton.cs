using System.Drawing;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    public partial class DBIconButton : Button
    {
        public DBIconButton(Image img)
        {
            InitializeComponent();
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Palette.DarkGrayBlueWorkingArea;
            this.Size = new Size(12, 12);
            this.Image = new Bitmap(img, new Size(this.Width, this.Height));
            this.MouseEnter += DBIconButton_MouseEnter;
            this.MouseLeave += DBIconButton_MouseLeave;
        }

        /*Меняем цвет фона на стандартный при потере фокуса*/
        private void DBIconButton_MouseLeave(object sender, System.EventArgs e)
        {
            this.BackColor = Palette.DarkGrayBlueWorkingArea;
        }

        /*Меняем цвет фона при получении фокуса*/
        private void DBIconButton_MouseEnter(object sender, System.EventArgs e)
        {
            this.BackColor = Palette.LightFocusedButtonColor;
        }
    }
}
