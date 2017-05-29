using System.Drawing;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    public partial class DBGroupBox : GroupBox
    {
        public DBGroupBox()
        {
            BackColor = Palette.LightBorderColor;
            ForeColor = Palette.LightGrayTextColor;
        }

        public DBGroupBox(Size size, Point location)
        {
            this.Size = size;
            this.Location = location;
            BackColor = Palette.LightBorderColor;
            ForeColor = Palette.LightGrayTextColor;
        }

        public DBGroupBox(string text)
        {
            this.Text = text;
            BackColor = Palette.LightBorderColor;
            ForeColor = Palette.LightGrayTextColor;
        }

        public DBGroupBox (string text, Size size, Point location)
        {
            this.Text = text;
            this.Size = size;
            this.Location = location;
            BackColor = Palette.LightBorderColor;
            ForeColor = Palette.LightGrayTextColor;
        }
    }
}
