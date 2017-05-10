using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormTry_1
{
    public partial class MainServerForm : Form
    {
        #region Объявление переменных
        /*Коэффициент, на который умножаем ширину экрана
         для окна в свернутой форме*/
        private float minimizedRightFormCoef = 0.02f;

        /*Высота окна (не меняется)*/
        int rightFormHeight;

        /*Положение по оси Х для уменьшенного и увеличенного окна*/
        private int minimizedX;
        private int maximizedX;

        /*Определяет, занята ли сейчас форма изменением размера*/
        private bool formChanging = false;

        /*Минимальный и максимальный размер пункта меню*/
        private Size minItemSize;
        private Size maxItemSize;

        /*Делегат для обработчиков нажатий на пункты меню*/
        private delegate MouseEventHandler ControlMouseClick(object sender, EventArgs e);

        /*Список пунктов меню*/
        private List<MenuItemControl> menuItems;

        /*Переменная, отвечающая за соединение*/
        RemoteConnection connection;

        #endregion

        #region Окна

        /*Окно чата*/
        ChatForm chat;

        /*Окно настроек*/
        PropertiesForm properties;

        #endregion

        #region Настройки формы

        public MainServerForm()
        {
            InitializeComponent();
            /*Получаем высоту формы*/
            this.rightFormHeight = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Height / 2);
            /*Задаем размер формы*/
            this.MaximumSize = new Size(120, rightFormHeight);
            this.MinimumSize = new Size(Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width *
                minimizedRightFormCoef), rightFormHeight);
            this.Size = MinimumSize;
            /*Устанавливаем расположение формы*/
            minimizedX = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width - MinimumSize.Width);
            maximizedX = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width - MaximumSize.Width);
            /*Располагаем форму по центру*/
            this.Location = new Point(Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width - this.Width),
                Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Height / 2 - Size.Height / 2));
            /*Устанавливаем цвет окна*/
            this.BackColor = Global.baseWindowColor;
            this.TopMost = true;

            /*Добавляем события формы*/
            this.FormClosing += MainServerForm_FormClosing;
            this.Load += MainServerForm_Load;
        }

        /*Загрузка формы. Инициализируем список объектов меню и выводим их*/
        private void MainServerForm_Load(object sender, EventArgs e)
        {
            int minVal = Convert.ToInt32(MinimumSize.Width * 0.8);
            int maxVal = Convert.ToInt32(MaximumSize.Width * 0.8);
            minItemSize = new Size(minVal, minVal);
            maxItemSize = new Size(maxVal, maxVal);
            menuItems = new List<MenuItemControl>();
            int itemLocationX = Convert.ToInt32(0.1 * MinimumSize.Width);
            #region Инициализация и добавление контролов
            /*Добавляем в список все пункты меню*/
            menuItems.Add(new AltActionsControl("Старт", Properties.Resources.start_icon,
                "Пауза", Properties.Resources.stop_icon));
            menuItems[0].MouseClick += StartStop_MouseClick;
            menuItems[0].iconBox.MouseClick += StartStop_MouseClick;
            menuItems[0].itemNameBox.MouseClick += StartStop_MouseClick;
            menuItems.Add(new MenuItemControl("Снимок экрана", Properties.Resources.screen_capture_icon));
            menuItems[1].MouseClick += ScreenCapture_MouseClick;
            menuItems[1].iconBox.MouseClick += ScreenCapture_MouseClick;
            menuItems[1].itemNameBox.MouseClick += ScreenCapture_MouseClick;
            menuItems.Add(new MenuItemControl("Чат", Properties.Resources.chat_icon));
            menuItems[2].MouseClick += Chat_MouseClick;
            menuItems[2].iconBox.MouseClick += Chat_MouseClick;
            menuItems[2].itemNameBox.MouseClick += Chat_MouseClick;
            menuItems.Add(new MenuItemControl("Позвонить", Properties.Resources.call_icon));
            /*menuItems[3].MouseClick += StartStop_MouseClick;
            menuItems[3].iconBox.MouseClick += StartStop_MouseClick;
            menuItems[3].itemNameBox.MouseClick += StartStop_MouseClick;*/
            menuItems.Add(new MenuItemControl("Настройки", Properties.Resources.properties_icon));
            menuItems[4].MouseClick += Properties_MouseClick;
            menuItems[4].iconBox.MouseClick += Properties_MouseClick;
            menuItems[4].itemNameBox.MouseClick += Properties_MouseClick;
            menuItems.Add(new AltActionsControl("Развернуть", Properties.Resources.maximize_icon, "Скрыть", Properties.Resources.minimize_icon));
            menuItems[5].MouseClick += Resize_MouseClick;
            menuItems[5].iconBox.MouseClick += Resize_MouseClick;
            menuItems[5].itemNameBox.MouseClick += Resize_MouseClick;
            menuItems.Add(new MenuItemControl("Свернуть", Properties.Resources.hide_icon));
            menuItems[6].MouseClick += Hide_MouseClick;
            menuItems[6].iconBox.MouseClick += Hide_MouseClick;
            menuItems[6].itemNameBox.MouseClick += Hide_MouseClick;
            menuItems.Add(new MenuItemControl("Выход", Properties.Resources.exit_icon));
            menuItems[7].MouseClick += Close_MouseClick;
            menuItems[7].iconBox.MouseClick += Close_MouseClick;
            menuItems[7].itemNameBox.MouseClick += Close_MouseClick;
            /*Добавляем элементы списка*/
            int itemLocationY = itemLocationX * 3;
            foreach (MenuItemControl item in menuItems)
            {
                item.Size = minItemSize;
                this.Controls.Add(item);
                if (item.text != "Развернуть" & item.text != "Свернуть" & item.text != "Выход")
                {
                    item.Location = new Point(itemLocationX, itemLocationY);
                    itemLocationY += item.Height + itemLocationX * 3;
                }
            }
            int indexFromEnd = menuItems.Count - 1;
            itemLocationY = Convert.ToInt32(MinimumSize.Height * 0.98) - minItemSize.Height;
            menuItems[indexFromEnd].Location = new Point(itemLocationX, itemLocationY);
            indexFromEnd--;
            itemLocationY -= (minItemSize.Height + itemLocationX * 3);
            menuItems[indexFromEnd].Location = new Point(itemLocationX, itemLocationY);
            indexFromEnd--;
            itemLocationY -= (minItemSize.Height + itemLocationX * 3);
            menuItems[indexFromEnd].Location = new Point(itemLocationX, itemLocationY);
            #endregion

            /*Генерируем случайный пароль*/
            Global.securityCode = GetRandomCode(8);

            /*Инициализируем экранную переменную*/
            Global.screenActions = new ScreenActions();

            /*Инициализируем сетевую переменную*/
            connection = new RemoteConnection();

            /*Создание иконки рядом с панелью задач*/
            notifyIcon.Icon = Properties.Resources.notify_icon;
            /*Подгружаем окно чата*/
            chat = new ChatForm();
            /*Подгружаем окно настроек*/
            properties = new PropertiesForm();
        }

        /*Создаем плавную анимацию изменения размера формы с помощьюю таймера
          Принимает true, если форму нужно расширить, и false если сузить*/
        private void SetChangeFormTimer(bool bigger)
        {
            formChanging = true;
            Timer changeFormSizeTimer = new Timer();
            changeFormSizeTimer.Interval = 10;  //каждые 10мс
            int stepCount = 0;
            int maxSteps = 2;
            int sizeStep = (this.MaximumSize.Width - this.MinimumSize.Width) / maxSteps;
            int locationStep = (minimizedX - maximizedX) / maxSteps;
            /*Определяем, каким должен быть шаг*/
            if (!bigger)
            {
                sizeStep = -sizeStep;
                locationStep = -locationStep;
            }
            changeFormSizeTimer.Tick += new EventHandler((o, ev) =>
            {
                this.Location = new Point(this.Location.X - locationStep, this.Location.Y);
                this.Size = new Size(this.Size.Width + sizeStep, this.Size.Height);
                /*Редактируем размер иконок*/
                foreach (MenuItemControl item in menuItems)
                {
                    item.Width += sizeStep;
                    item.maximized = bigger;
                }
                stepCount++;
                /*Когда сделали максимальное число шагов*/
                if (stepCount == maxSteps)
                {
                    formChanging = false;
                    Location = new Point(bigger ? maximizedX : minimizedX, Location.Y);
                    changeFormSizeTimer.Stop();
                    changeFormSizeTimer.Dispose();
                }
            });
            changeFormSizeTimer.Start();
        }

        /*Задержка изменения размера на число мсек и выполнение операции*/
        public void DelayResize(int ms, bool val)
        {
            Timer delayTimer = new Timer();
            delayTimer.Interval = ms;
            delayTimer.Tick += new EventHandler((o, ev) =>
            {
                SetChangeFormTimer(val);
                delayTimer.Stop();
                delayTimer.Dispose();
            });
            delayTimer.Start();
        }
        #endregion
        
        #region Обработчики нажатий для контролов

        /*Включение/отключение сеанса*/
        private void StartStop_MouseClick(object sender, EventArgs e)
        {           
        }

        /*Снимок экрана*/
        private void ScreenCapture_MouseClick(object sender, EventArgs e)
        {
           //screenshot.Save("screenshotTry.jpg");        
        }

        private void Chat_MouseClick(object sender, EventArgs e)
        {
            if (chat.shown)
                chat.Hide();
            else
                chat.Show();
        }

        /*Изменение размеров формы*/
        private void Resize_MouseClick(object sender, EventArgs e)
        {
            /*Если панелька уменьшена, то разворачиваем ее*/
            if (Location.X != maximizedX)
            {
                /*Создаем плавную анимацию с помощьюю таймера*/
                if (formChanging)
                    DelayResize(1000, true);
                else
                    SetChangeFormTimer(true);
            }
            /*Иначе сворачиваем*/
            else
            {
                /*Создаем плавную анимацию с помощьюю таймера,
                  если форма сейчас не занята.
                  Если форма занята - ждем 1с и производим изменения*/
                if (formChanging)
                    DelayResize(1000, false);
                else
                    SetChangeFormTimer(false);
            }
        }

        /*Свернуть окно*/
        private void Hide_MouseClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = true;
            this.Hide();
        }

        /*Выход*/
        private void Close_MouseClick(object sender, EventArgs e)
        {
                this.Close();
        }

        /*Открытие окна "настройки"*/
        private void Properties_MouseClick(object sender, MouseEventArgs e)
        {
            properties.Show();
        }

        #endregion

        #region Обработчики событий формы

        private void MainServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = DialogForm.Show("Выход", "Завершить работу программы?", Global.DialogTypes.close);
            if (dialogResult == DialogResult.Yes)
            {
                notifyIcon.Dispose();
                chat.Dispose();
                properties.Dispose();
                this.Dispose();
            }
            else
                e.Cancel = true;
        }

        #endregion

        #region Работа с панелью задач

        /*Нажатие на иконку в трее (ПКМ)*/
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
                notifyMenu.Show();
        }

        /*Двойное нажатие на иконку в трее (ЛКМ)*/
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
                this.Show();
        }

        /*Выбор пункта "закрыть"*/
        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*Выбор пункта "Развернуть"*/
        private void showMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        /*Выход за границы менюхи*/
        private void notifyMenu_MouseCaptureChanged(object sender, EventArgs e)
        {
            notifyMenu.Hide();
        }

        #endregion

        #region Остальные методы

        /*Генерация рандомного пароля*/
        public String GetRandomCode(int size)
        {
            string password = "";
            Random rand = new Random();
            char c;
            while(password.Length<size)
            {
                c = (char)rand.Next(33, 125);
                if (Char.IsLetterOrDigit(c))
                    password += c;
            }
            return password;
        }

        #endregion




    }
}
