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
    /*Шаблон для всех (кроме главной) форм проекта. Включает в себя 
     заголовок, обработку общих событий, настройки внешнего вида*/
    public partial class ChildFormsTemplate : Form
    {
        /*Заголовок*/
        protected FormBorders header;

        /*Отображается ли сейчас форма*/
        public bool shown = false;

        /*Переопределяем ClientRectangle для нашего окна, учитывая заголовок*/
        public new Rectangle ClientRectangle
        {
            get
            {
                if (header != null)
                    return new Rectangle(1, header.Height, Width, Height - (header.Height + 1));
                return Rectangle.Empty;
            }
            set { }
        }

        public ChildFormsTemplate()
        {
            InitializeComponent();
            /*Настраиваем форму*/
            this.BackColor = Global.baseWindowColor;
            this.Width = 300;
            this.Height = 400;
        }

        /*Загрузка формы*/
        private void ChildFormsTemplate_Load(object sender, EventArgs e)
        {
            /*Заголовок окна*/
            header = new FormBorders(this);
            Controls.Add(header);
            DoubleBuffered = true;
            this.VisibleChanged += ChildFormsTemplate_VisibleChanged;
            this.FormClosing += ChildFormsTemplate_FormClosing;
        }

        protected void ChildFormsTemplate_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        /*При отображении/скрытии формы*/
        protected void ChildFormsTemplate_VisibleChanged(object sender, EventArgs e)
        {
            shown = !shown;
        }
    }
}
