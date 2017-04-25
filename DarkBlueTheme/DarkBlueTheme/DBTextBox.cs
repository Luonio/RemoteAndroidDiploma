using System.Drawing;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    public partial class DBTextBox : TextBox
    {
        string initialText = "";

        /*Общие поля ввода*/
        public DBTextBox()
        {
            InitializeComponent();
            this.BorderStyle = BorderStyle.None;
            this.BackColor = Color.FromArgb(200, 200, 200);
            this.AcceptsReturn = false;
        }

        /*Поля ввода данных*/
        public DBTextBox(string initialText)
        {
            InitializeComponent();
            this.Size = new Size(120, this.Height);
            this.BackColor = Color.FromArgb(200, 200, 200);
            this.BorderStyle = BorderStyle.None;
            this.AcceptsReturn = false;
            this.initialText = initialText;
            this.Text = initialText;
            this.ForeColor = Color.FromArgb(150, 150, 150);
            this.GotFocus += DBTextBox_GotFocus;
            this.LostFocus += DBTextBox_LostFocus;
        }

        private void DBTextBox_LostFocus(object sender, System.EventArgs e)
        {
            if (this.Text == "")
            {
                this.Text = initialText;
                this.ForeColor = Color.FromArgb(150, 150, 150);
            }
        }

        /*Когда получаем фокус*/
        private void DBTextBox_GotFocus(object sender, System.EventArgs e)
        {
            if (this.Text == initialText)
            {
                this.Text = "";
                this.ForeColor = Color.Black;
            }
        }
    }
}
