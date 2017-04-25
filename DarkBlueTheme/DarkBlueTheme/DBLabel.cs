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
            this.ForeColor = Color.FromArgb(230, 255, 255);
            this.AutoSize = true;
        }

        public DBLabel(string text)
        {
            InitializeComponent();
            this.Text = text;
            this.ForeColor = Color.FromArgb(230, 255, 255);
            this.AutoSize = true;

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
