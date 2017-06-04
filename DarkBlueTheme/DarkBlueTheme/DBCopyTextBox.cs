using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System;

namespace DarkBlueTheme
{
    public partial class DBCopyTextBox : TextBox
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        public DBCopyTextBox()
        {
            InitializeComponent();
            this.BackColor = Palette.DarkGrayBlueWorkingArea;
            this.ForeColor = Palette.LightGrayTextColor;
            this.ReadOnly = true;
        }

        public DBCopyTextBox(string text)
        {
            InitializeComponent();
            /*Устанавливаем боксу ширину строки*/
            this.Width = TextRenderer.MeasureText(text, this.Font).Width;
            this.BackColor = Palette.DarkGrayBlueWorkingArea;
            this.ForeColor = Palette.LightGrayTextColor;
            this.BorderStyle = BorderStyle.None;
            this.ReadOnly = true;
            this.Cursor = Cursors.Arrow;
            this.Text = text;
            this.HideSelection = true;
            this.GotFocus += DBCopyTextBox_GotFocus;
            this.TextChanged += DBCopyTextBox_TextChanged;
        }

        /*При изменении текста пересчитываем его ширину*/
        private void DBCopyTextBox_TextChanged(object sender, EventArgs e)
        {
            this.Width = TextRenderer.MeasureText(this.Text, this.Font).Width;
        }

        /*Скрываем курсор, так как нам нужно только вывести данные*/
        public void HideCaret()
        {
            HideCaret(this.Handle);
        }

        private void DBCopyTextBox_GotFocus(object sender, System.EventArgs e)
        {
            HideCaret();
        }
    }
}
