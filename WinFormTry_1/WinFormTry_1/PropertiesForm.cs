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
        TransparentTabControl pages;

        public PropertiesForm()
        {
            InitializeComponent();
            this.Text = "Настройки";
            this.Icon = Properties.Resources.header_properties;
            this.BackColor = Global.baseWindowColor;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        private void PropertiesForm_Load(object sender, EventArgs e)
        {
            #region Добавляем контролы
            FormBorders header = new FormBorders(this);
            Controls.Add(header);
            pages = new TransparentTabControl();
            pages.Size = new Size(ClientRectangle.Width + 1, ClientRectangle.Height + 1);
            pages.Location = new Point(0, ClientRectangle.Y);
            /*Настраиваем вкладку "персональные"*/
            pages.TabPages.Add(new TabPage("Персональные"));
            pages.TabPages.Add(new TabPage("Соединение"));
            pages.TabPages.Add(new TabPage("Общие"));
            Controls.Add(pages);
            pages.MakeTransparent();
            #endregion
        }
    }

    /*Прозрачный TabControl*/
    class TransparentTabControl : TabControl
    {
        private List<Panel> pages = new List<Panel>();

        public void MakeTransparent()
        {
            if (TabCount == 0)
                throw new InvalidOperationException();
            var height = GetTabRect(0).Bottom;
            /*Перемещаем контролы на панель*/
            for (int tab = 0; tab < TabCount; ++tab)
            {
                var page = new Panel
                {
                    Left = this.Left,
                    Top = this.Top + height,
                    Width = this.Width,
                    Height = this.Height - height,
                    BackColor = Color.Transparent,
                    Visible = tab == this.SelectedIndex
                };
                for (int ix = TabPages[tab].Controls.Count - 1; ix >= 0; --ix)
                {
                    TabPages[tab].Controls[ix].Parent = page;
                }
                pages.Add(page);
                this.Parent.Controls.Add(page);
            }
            this.Height = height;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            for (int tab = 0; tab < pages.Count; ++tab)
            {
                pages[tab].Visible = tab == SelectedIndex;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                foreach (var page in pages)
                    page.Dispose();
            base.Dispose(disposing);
        }
    }
}
