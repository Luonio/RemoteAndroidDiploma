using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormTry_1
{
    public partial class PropertiesForm : ChildFormsTemplate
    {
        public PropertiesForm()
        {
            InitializeComponent();
            this.Text = "Настройки";
        }

        private void PropertiesForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.header_properties;
            this.BackColor = Global.baseWindowColor;
            #region Добавляем контролы
            FormBorders header = new FormBorders(this);
            Controls.Add(header);
            TabControl pages = new TabControl();
            pages.TabPages.Add(new TabPage("Персональные"));
            pages.TabPages.Add(new TabPage("Соединение"));
            pages.TabPages.Add(new TabPage("Общие"));
            #endregion
        }
    }
}
