using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DarkBlueTheme;

namespace WinFormTry_1
{
    public partial class InitialForm : WinFormTry_1.ChildFormsTemplate
    {
        DBTextBox nameBox;
        DBTextBox ipBox;
        DBTextBox codeBox;

        MainServerForm mainForm;

        public InitialForm()
        {
            InitializeComponent();
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += InitialForm_Load;
        }

        private void InitialForm_Load(object sender, EventArgs e)
        {
            #region Настраиваем контролы
            nameBox = new DBTextBox("Имя пользователя");
            nameBox.Location = new Point(this.ClientRectangle.X+20, this.ClientRectangle.Y+30);
            this.Controls.Add(nameBox);
            ipBox = new DBTextBox("Удаленный IP");
            ipBox.Location = new Point(this.ClientRectangle.X + 20, this.ClientRectangle.Y + 60);
            this.Controls.Add(ipBox);
            codeBox = new DBTextBox("Код доступа");
            codeBox.Location = new Point(this.ClientRectangle.X + 40 + ipBox.Width, this.ClientRectangle.Y + 45);
            this.Controls.Add(codeBox);
            DBButton OKButton = new DBButton
            {
                Location = new Point(ClientRectangle.Width - 85, Height - 30),
                Text = "ОК"
            };
            this.Controls.Add(OKButton);
            OKButton.MouseClick += OKButton_MouseClick;
            #endregion

        }

        private void OKButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (mainForm == null)
            {
                mainForm = new MainServerForm();
                mainForm.Show();
            }
        }
    }
}
