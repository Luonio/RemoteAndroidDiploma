using System;
using System.Drawing;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    
    /*Шаблон формы. Включает в себя заголовок, обработку общих
      событий, настройки внешнего вида*/
    public partial class DBForm : Form
    {
        #region Поля и свойства
        /*Заголовок*/
        protected DBHeader Header;

        /*Рабочее пространство формы*/
        public DBPanel WorkingArea;

        /*true - форма открыта, false - форма скрыта*/
        public bool IsShown = false;

        /*Переопределяем ClientRectangle для нашего окна, учитывая заголовок*/
        public new Rectangle ClientRectangle
        {
            get
            {
                if (Header != null)
                    return new Rectangle(1, Header.Height + 1, Width-1, Height - (Header.Height + 2));
                return Rectangle.Empty;
            }
            set { }
        }
        #endregion

        #region Конструкторы
        public DBForm()
        {
            InitializeComponent();
            /*Настраиваем форму*/
            MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true;
            BackColor = Palette.DarkBlue;
            Width = 300;
            Height = 400;
            Header = new DBHeader(this);
            WorkingArea = new DBPanel(this);
            Controls.Add(Header);
            Controls.Add(WorkingArea);
            /*Добавляем события*/
            VisibleChanged += DBForm_VisibleChanged;
            Resize += DBForm_Resize;
            FormClosing += DBForm_FormClosing;
        }
        #endregion

        #region События формы/

        /*Событие по отрисовке формы*/
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawBorders();
        }

        /*При отображении/скрытии формы*/
        public void DBForm_VisibleChanged(object sender, EventArgs e)
        {
            IsShown = !IsShown;
        }

        private void DBForm_Resize(object sender, EventArgs e)
        {
            Header.SetBounds();
            WorkingArea.SetBounds();
            Refresh();
        }

        public virtual void DBForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        #endregion

        #region Методы 
        /*Рисование рамки на форме*/
        private void DrawBorders()
        {
            /*Добавляем рамку*/
            Graphics clientRect = CreateGraphics();
            Pen borderPen = new Pen(Palette.LightBorderColor);
            clientRect.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            clientRect.Dispose();
        }
        #endregion
    }
}
