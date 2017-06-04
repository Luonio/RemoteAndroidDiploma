using System;
using System.Drawing;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    public partial class DBMessagingBox : TableLayoutPanel
    {
        public DBMessagingBox()
        {
            InitializeComponent();
            AutoScroll = false;
            HorizontalScroll.Enabled = false;
            HorizontalScroll.Visible = false;
            HorizontalScroll.Maximum = 0;
            VerticalScroll.Enabled = false;
            VerticalScroll.Visible = false;
            VerticalScroll.Maximum = 0;

            AutoScroll = true;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute));
            this.Resize += DBMessagingBox_Resize;
        }

        private void DBMessagingBox_Resize(object sender, EventArgs e)
        {
            ColumnStyles[0].Width = this.Width / 2-5;
            ColumnStyles[1].Width = this.Width / 2-5;
        }
    }
}
