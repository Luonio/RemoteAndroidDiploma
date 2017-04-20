using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormTry_1
{
    public partial class MenuItemControl : UserControl
    {
        public String text;
        public Bitmap img;
        public bool maximized = false;

        public MenuItemControl()
        {
            this.img = null;
            this.text = "";
            InitializeComponent();
        }

        public MenuItemControl(string txt, Bitmap bmp)
        {          
            img = bmp;
            this.text = txt;
            InitializeComponent();
            iconBox.Image = img;
        }

        private void MenuItemControl_Load(object sender, EventArgs e)
        {
            /*Задаем стиль контрола*/
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            this.ForeColor = Global.itemTextColor;
            itemNameBox.Location = new Point(iconBox.Width + Convert.ToInt32(0.3 * this.Width), 
                iconBox.Height / 2 - itemNameBox.Height/2);
            itemNameBox.Text = this.text;
            itemNameBox.Hide();
        }

        protected void MenuItemControl_Resize(object sender, EventArgs e)
        {
            iconBox.Width = Convert.ToInt32(this.Height * 0.8);
            iconBox.Height = Convert.ToInt32(this.Height * 0.8);
            iconBox.Location = new Point(iconBox.Location.X, this.Height / 2 - iconBox.Height / 2);
            if (maximized)
                itemNameBox.Show();
            else
                itemNameBox.Hide();
        }


        private void MenuItemControl_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Global.selectedItemColor;
            this.ForeColor = Global.selectedItemTextColor;
        }

        private void MenuItemControl_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Global.menuItemColor;
            this.ForeColor = Global.itemTextColor;
        }

        private void MenuItemControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.BackColor = Global.clickedItemColor;
            this.ForeColor = Global.clickedItemTextColor;
        }

        private void MenuItemControl_MouseUp(object sender, MouseEventArgs e)
        {
            this.BackColor = Global.selectedItemColor;
        }
        /*Вывод всплывающей подсказки по положению курсора*/
        private void iconBox_MouseHover(object sender, EventArgs e)
        {
            this.funcNameTip.Show(this.itemNameBox.Text, iconBox);
        }
    }    
}
