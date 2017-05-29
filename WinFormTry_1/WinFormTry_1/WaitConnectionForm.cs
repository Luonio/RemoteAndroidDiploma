using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkBlueTheme;

namespace WinFormTry_1
{
    public partial class WaitConnectionForm : ChildFormsTemplate
    {
        #region Поля
        /*Контролы*/
        DBCopyTextBox username;
        DBCopyTextBox ip;
        DBCopyTextBox pass;

        DBIconButton changeUsernameButton;
        DBIconButton refreshPassButton;

        MainServerForm server;

        #endregion

        #region Конструкторы
        public WaitConnectionForm()
        {
            InitializeComponent();
            this.Text = "Подключение";
            /*Располагаем форму по центру*/
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = Properties.Resources.notify_icon;
        }
        #endregion

        #region События формы
        /*Загрузка формы*/
        private void WaitConnectionForm_Load(object sender, EventArgs e)
        {
            int marginCoef = 10;
            /*Генерируем случайный пароль*/
            Global.securityCode = GetRandomCode(8);
            /*Инициализируем экранную переменную*/
            Global.screenActions = new ScreenActions();
            #region Контролы
            /*Label'ы*/
            DBLabel nameLabel = new DBLabel("Имя пользователя:");
            DBLabel ipLabel = new DBLabel("Текущий IP-адрес:");
            DBLabel passLabel = new DBLabel("Пароль:");
            username = new DBCopyTextBox(Global.username.ToString());
            ip = new DBCopyTextBox(Global.hostIP);
            pass = new DBCopyTextBox(Global.securityCode);
            nameLabel.Location = new Point(marginCoef, ClientRectangle.Y + marginCoef*2);
            ipLabel.Location = new Point(marginCoef, nameLabel.Location.Y + nameLabel.Height + marginCoef);
            passLabel.Location = new Point(marginCoef, ipLabel.Location.Y + ipLabel.Height + marginCoef);
            username.Location = new Point(nameLabel.Width + marginCoef, nameLabel.Location.Y);
            ip.Location = new Point(ipLabel.Width + marginCoef, ipLabel.Location.Y);
            pass.Location = new Point(passLabel.Width + marginCoef, passLabel.Location.Y);
            Controls.Add(nameLabel);
            Controls.Add(ipLabel);
            Controls.Add(passLabel);
            Controls.Add(username);
            Controls.Add(ip);
            Controls.Add(pass);
            /*Кнопки редактирования*/
            changeUsernameButton = new DBIconButton(Properties.Resources.edit_icon);
            changeUsernameButton.Location = new Point(username.Location.X + username.Width, username.Location.Y);
            refreshPassButton = new DBIconButton(Properties.Resources.refresh_icon);
            refreshPassButton.Location = new Point(pass.Location.X + pass.Width, pass.Location.Y);
            /*По нажатию на кнопку обновляем пароль*/
            refreshPassButton.MouseClick += RefreshPassButton_MouseClick;
            Controls.Add(changeUsernameButton);
            Controls.Add(refreshPassButton);
            #endregion
            server = new MainServerForm();
            server.properties = this;
            /*Инициализируем сетевую переменную*/
            Global.connection = new RemoteConnection();
            /*Заводим таймер на отслеживание подключения*/
            Timer connectionMonitor = new Timer();
            connectionMonitor.Interval = 100;
            connectionMonitor.Tick += ((o, ev) =>
            {
                if (Global.connection.Connected)
                    server.Show();
                else
                {
                    server.Hide();
                    this.Show();
                }
            });
            connectionMonitor.Start();
        }

        protected override void ChildFormsTemplate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Global.connection.Connected)
            {
                DialogResult dialogResult = DialogForm.Show("Выход", "Завершить работу программы?", Global.DialogTypes.close);
                if (dialogResult == DialogResult.Yes)
                    this.Dispose();
                else
                    e.Cancel = true;
            }
            else
            {
                e.Cancel = true;
                Hide();
            }

        }

        /*При нажатии на кнопку обновления пароля, генерируем пароль заново и выводим его*/
        private void RefreshPassButton_MouseClick(object sender, MouseEventArgs e)
        {
            /*Генерируем случайный пароль*/
            Global.securityCode = GetRandomCode(8);
            pass.Text = Global.securityCode.ToString();
            refreshPassButton.Location = new Point(pass.Location.X + pass.Width, pass.Location.Y);
        }
        #endregion

        #region Методы
        /*Генерация рандомного пароля*/
        public String GetRandomCode(int size)
        {
            string password = "";
            Random rand = new Random();
            char c;
            while (password.Length < size)
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
