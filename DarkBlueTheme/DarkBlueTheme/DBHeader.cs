using System;
using System.Drawing;
using System.Windows.Forms;

namespace DarkBlueTheme
{
    public partial class DBHeader : UserControl
    {
        /*Ссылка на родительскую форму*/
        Form parentForm;

        /*Кнопки*/
        Button exitButton;
        Button resizeButton;
        Button hideButton;

        /*Заголовок окна*/
        Label headerBox;

        /*невидимые контролы для масштабирования*/
        struct ScalingControls
        {
            public static Control topLeft;
            public static Control top;
            public static Control topRight;
            public static Control right;
            public static Control bottomRight;
            public static Control bottom;
            public static Control bottomLeft;
            public static Control left;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        /*Индексы метрик для функции*/
        public struct Indexes
        {
            /*Ширина границы окна*/
            public static int SM_CYBORDER = 6;
            /*Высота заголовка окна*/
            public static int SM_CYCAPTION = 4;
            /*Ширина и высота кнопок заголовка*/
            public static int SM_CXSIZE = 30;
            public static int SM_CYSIZE = 31;
            /*Размеры иконки заголовка окна*/
            public static int SM_CXSMICON = 49;
            public static int SM_CYSMICON = 50;
        }

        /*Типы кнопок на контроле*/
        public enum ButtonTypes
        {
            exit = 0,
            hide = 1,
            resize = 2
        }

        #region Переменные
        /*Отвечает за рисование рамки*/
        bool isMaximized = false;
        /*Сторона квадрата, в котором находится картинка на кнопке*/
        int pictSide;
        /*Предыдущее положение курсора*/
        private Point prevMousePosition;
        /*Новое положение курсора*/
        private Point prevFormPosition;

        /*Переменная для хранения размера формы до ее увеличения на весь экран*/
        private Point preResizeFormPosition;
        #endregion



        public DBHeader(Form frm)
        {
            InitializeComponent();
            parentForm = frm;
            this.Width = frm.ClientSize.Width-2;
            this.Height = GetSystemMetrics(Indexes.SM_CYCAPTION) + 7;
            this.Location = new Point(1, 1);
            this.BackColor = Palette.DarkBlue;
            this.ForeColor = Palette.LightGrayTextColor;
            this.Load += DBHeader_Load;
            this.MouseDown += DBHeader_MouseDown;
            this.MouseMove += DBHeader_MouseMove;
            this.Paint += DBHeader_Paint;
        }

        private void DBHeader_Load(object sender, EventArgs e)
        {
            #region Добавление элементов
            /*Отображаемые элементы*/
            AddButton(ButtonTypes.exit);
            AddButton(ButtonTypes.resize);
            AddButton(ButtonTypes.hide);
            AddText();
            /*Контролы масштабирования*/
            ScalingControls.topLeft = new Control();
            ScalingControls.top = new Control();
            ScalingControls.topRight = new Control();
            ScalingControls.right = new Control();
            ScalingControls.bottomRight = new Control();
            ScalingControls.bottom = new Control();
            ScalingControls.bottomLeft = new Control();
            ScalingControls.left = new Control();
            #endregion
            this.DoubleBuffered = true;
            parentForm.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            parentForm.FormBorderStyle = FormBorderStyle.None;
            /*Добавляем отлов изменения размера родительской формы*/
            parentForm.Resize += ParentForm_OnResize;
            /*Добавляем onPaint для родительской формы*/
            parentForm.Paint += parentForm_OnPaint;
            this.Show();
            parentForm.Activated += ParentForm_Activated;
        }

        private void ParentForm_Activated(object sender, EventArgs e)
        {
            DrawBorders();
        }

        /*Добавление на контрол кнопок*/
        private void AddButton(ButtonTypes type)
        {
            Button bt = new Button();
            bt.Size = new Size(GetSystemMetrics(Indexes.SM_CXSIZE), this.Height);
            bt.Width -= 1;
            bt.BackgroundImageLayout = ImageLayout.Zoom;
            bt.BackColor = Color.Transparent;
            bt.FlatStyle = FlatStyle.Flat;
            bt.FlatAppearance.BorderSize = 0;
            /*В зависимости от вида кнопки устанавливаем ее положение на контроле
              и  изображение, добавляем обработчики нажатий*/
            pictSide = (Convert.ToInt32(bt.Height * 0.7));
            switch (type)
            {
                case ButtonTypes.exit:
                    bt.Location = new Point(this.Width - bt.Width, 0);
                    bt.Image = new Bitmap(Properties.Resources.close_window_icon,
                        new Size(pictSide, pictSide));
                    bt.MouseClick += exitButton_OnClick;
                    bt.MouseEnter += exitButton_MouseEnter;
                    bt.MouseLeave += exitButton_MouseLeave;
                    exitButton = bt;
                    break;
                case ButtonTypes.resize:
                    bt.Location = new Point(this.Width - 2 * bt.Width, 0);
                    bt.Image = new Bitmap(Properties.Resources.resize_window_icon,
                        new Size(pictSide, pictSide));
                    bt.MouseClick += resizeButton_OnClick;
                    bt.MouseEnter += resizeButton_MouseEnter;
                    bt.MouseLeave += resizeButton_MouseLeave;
                    resizeButton = bt;
                    break;
                case ButtonTypes.hide:
                    bt.Location = new Point(this.Width - 3 * bt.Width, 0);
                    bt.Image = new Bitmap(Properties.Resources.hide_window_icon,
                        new Size(pictSide, pictSide));
                    bt.MouseClick += hideButton_MouseClick;
                    bt.MouseEnter += hideButton_MouseEnter;
                    bt.MouseLeave += hideButton_MouseLeave;
                    hideButton = bt;
                    break;
            }
            this.Controls.Add(bt);
        }

        /*Добавление текста заголовка окна*/
        private void AddText()
        {
            headerBox = new Label();
            headerBox.Text = ParentForm.Text;
            headerBox.Location = new Point(Convert.ToInt32(this.Height), this.Height / 2 - headerBox.Height / 4);
            headerBox.BackColor = Color.Transparent;
            headerBox.MouseDown += DBHeader_MouseDown;
            headerBox.MouseMove += DBHeader_MouseMove;
            this.Controls.Add(headerBox);
        }

        #region События перерисовки/масштабирования

        /*При изменении размеров родительской формы меняем размер контрола
         и положение кнопок*/
        private void ParentForm_OnResize(object sender, EventArgs e)
        {
            this.Width = Parent.Width-2;
            exitButton.Location = new Point(this.Width - exitButton.Width, 0);
            resizeButton.Location = new Point(this.Width - 2 * resizeButton.Width, 0);
            hideButton.Location = new Point(this.Width - 3 * hideButton.Width, 0);
            parentForm.Invalidate();

        }

        /*Добавляем рамку*/
        private void parentForm_OnPaint(object sender, PaintEventArgs e)
        {
            if (this.parentForm.WindowState == FormWindowState.Normal)
                DrawBorders();
        }

        /*Рисование рамки на родительской форме*/
        private void DrawBorders()
        {
            /*Добавляем рамку*/
            Graphics clientRect = parentForm.CreateGraphics();
            Pen borderPen = new Pen(Palette.LightBorderColor);
            clientRect.DrawRectangle(borderPen, 0, 0, parentForm.Width - 1, parentForm.Height - 1);
            clientRect.Dispose();
        }

        private void DBHeader_Paint(object sender, PaintEventArgs e)
        {
            Graphics clientRect = this.CreateGraphics();
            /*Рисуем иконку*/
            Rectangle iconBox = new Rectangle(new Point(Convert.ToInt32(this.Height * 0.3), Convert.ToInt32(this.Height * 0.2)),
               new Size(Convert.ToInt32(this.Height * 0.6), Convert.ToInt32(this.Height * 0.6)));
            clientRect.DrawIcon(parentForm.Icon, iconBox);
            clientRect.DrawRectangle(Pens.Transparent, iconBox);
            clientRect.Dispose();
        }

        #endregion

        #region Обработчики, отвечающие за отображение кнопок

        /*При потере фокуса кнопкой выход*/
        private void exitButton_MouseLeave(object sender, EventArgs e)
        {
            exitButton.BackColor = Color.Transparent;
            exitButton.Image.Dispose();
            exitButton.Image = new Bitmap(Properties.Resources.close_window_icon,
                        new Size(pictSide, pictSide));
        }

        /*Наведение мыши на кнопку выход*/
        private void exitButton_MouseEnter(object sender, EventArgs e)
        {
            exitButton.BackColor = Color.Red;
            exitButton.Image.Dispose();
            exitButton.Image = new Bitmap(Properties.Resources.close_window_icon,
                        new Size(pictSide, pictSide));
        }

        private void hideButton_MouseLeave(object sender, EventArgs e)
        {
            hideButton.BackColor = Color.Transparent;
        }

        private void hideButton_MouseEnter(object sender, EventArgs e)
        {
            hideButton.BackColor = Palette.LightFocusedButtonColor;
        }

        private void resizeButton_MouseLeave(object sender, EventArgs e)
        {
            resizeButton.BackColor = Color.Transparent;
        }

        private void resizeButton_MouseEnter(object sender, EventArgs e)
        {
            resizeButton.BackColor = Palette.LightFocusedButtonColor;
        }

        #endregion

        #region Обработчики нажатий на кнопки

        /*Нажатие на кнопку свернуть*/
        private void hideButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.parentForm.WindowState = FormWindowState.Minimized;
        }

        /*Нажатие на кнопку выход*/
        private void exitButton_OnClick(object sender, MouseEventArgs e)
        {
            this.parentForm.Close();
        }

        /*Нажатие на кнопку изменения размера*/
        private void resizeButton_OnClick(object sender, MouseEventArgs e)
        {
            /*Разворачиваем форму на весь экран, сохраняя в переменную
              ее положение до изменения*/
            if (this.parentForm.WindowState == FormWindowState.Normal)
            {
                preResizeFormPosition = parentForm.Location;
                parentForm.WindowState = FormWindowState.Maximized;
                resizeButton.Image.Dispose();
                resizeButton.Image = new Bitmap(Properties.Resources.toNormal_window_icon,
                        new Size(pictSide, pictSide));
            }
            /*Сворачиваем форму до прежних размеров, возвращая ее на прежнюю позицию*/
            else
            {
                this.parentForm.WindowState = FormWindowState.Normal;
                resizeButton.Image.Dispose();
                parentForm.Location = preResizeFormPosition;
                resizeButton.Image = new Bitmap(Properties.Resources.resize_window_icon,
                        new Size(pictSide, pictSide));
            }
        }

        #endregion

        #region Перетаскивание родительской формы

        /*Движение мыши в области заголовка*/
        private void DBHeader_MouseMove(object sender, MouseEventArgs e)
        {
            /*Если кнопка нажата - перетаскиваем форму*/
            if (e.Button == MouseButtons.Left)
            {
                Point newMousePosition = MousePosition;
                parentForm.Location = new Point(prevFormPosition.X + (newMousePosition.X - prevMousePosition.X),
                    prevFormPosition.Y + (newMousePosition.Y - prevMousePosition.Y));
                prevFormPosition = parentForm.Location;
                prevMousePosition = newMousePosition;
            }
        }

        /*Сохраняем место нажатия кнопки и положение формы для перетаскивания*/
        private void DBHeader_MouseDown(object sender, MouseEventArgs e)
        {
            prevMousePosition = MousePosition;
            prevFormPosition = parentForm.Location;
        }
        #endregion       
    }
}
