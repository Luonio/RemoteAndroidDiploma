using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    public partial class DBLabel : Label
    { 

        public DBLabel()
        {
            InitializeComponent();
            this.ForeColor = Palette.LightGrayTextColor;
            this.AutoSize = true;
        }

        public DBLabel(string text)
        {
            InitializeComponent();
            /*Устанавливаем метке ширину строки*/
            this.Width = TextRenderer.MeasureText(text, this.Font).Width;
            this.Text = text;
            this.ForeColor = Palette.LightGrayTextColor;
            this.AutoSize = true;

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
