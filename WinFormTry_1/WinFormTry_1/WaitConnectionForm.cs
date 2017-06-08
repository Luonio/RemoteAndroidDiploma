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
    public partial class WaitConnectionForm : DBForm
    {
        #region Поля
        /*Контролы*/
        DBCopyTextBox username;
        DBCopyTextBox address;
        DBCopyTextBox pass;

        DBIconButton changeUsernameButton;
        DBIconButton changePortButton;
        DBIconButton refreshPassButton;

        MainServerForm server;

        #endregion

        #region Конструкторы
        public WaitConnectionForm()
        {
            InitializeComponent();
            this.Size = new Size(300, 400);
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
            this.Height = 200;
            /*Генерируем случайный пароль*/
            Global.securityCode = GetRandomCode(8);
            /*Инициализируем экранную переменную*/
            Global.screenActions = new ScreenActions();
            #region Контролы
            /*Label'ы*/
            DBLabel nameLabel = new DBLabel("Имя пользователя:");
            DBLabel ipLabel = new DBLabel("Текущий адрес:");
            DBLabel passLabel = new DBLabel("Пароль:");
            username = new DBCopyTextBox(Global.username.ToString());
            address = new DBCopyTextBox(String.Format(Global.externalIP+":{0}",Global.receivePort));
            pass = new DBCopyTextBox(Global.securityCode);
            nameLabel.Location = new Point(marginCoef, marginCoef*2);
            ipLabel.Location = new Point(marginCoef, nameLabel.Location.Y + nameLabel.Height + marginCoef);
            passLabel.Location = new Point(marginCoef, ipLabel.Location.Y + ipLabel.Height + marginCoef);
            username.Location = new Point(nameLabel.Width + marginCoef, nameLabel.Location.Y);
            address.Location = new Point(ipLabel.Width + marginCoef, ipLabel.Location.Y);
            pass.Location = new Point(passLabel.Width + marginCoef, passLabel.Location.Y);
            WorkingArea.Controls.Add(nameLabel);
            WorkingArea.Controls.Add(ipLabel);
            WorkingArea.Controls.Add(passLabel);
            WorkingArea.Controls.Add(username);
            WorkingArea.Controls.Add(address);
            WorkingArea.Controls.Add(pass);
            /*Кнопки редактирования*/
            changeUsernameButton = new DBIconButton(Properties.Resources.edit_icon);
            changeUsernameButton.Location = new Point(username.Location.X + username.Width, username.Location.Y);
            changePortButton = new DBIconButton(Properties.Resources.edit_icon);
            changePortButton.Location = new Point(address.Location.X + address.Width, address.Location.Y);
            refreshPassButton = new DBIconButton(Properties.Resources.refresh_icon);
            refreshPassButton.Location = new Point(pass.Location.X + pass.Width, pass.Location.Y);
            /*По нажатию на кнопку вызываем диалог изменения свойства*/
            changeUsernameButton.Click += ((o, ev) =>
              {
                  DialogResult result = new DialogResult();
                  string editField = DialogForm.Show("Имя пользователя", Global.username, Global.DialogTypes.edit, out result);
                  if (result == DialogResult.OK)
                      Global.username = editField;
                  username.Text = Global.username;
                  changeUsernameButton.Location = new Point(username.Location.X + username.Width, username.Location.Y);
              });
            changePortButton.Click += ((o, ev) =>
              {
                  DialogResult result = new DialogResult();
                  string editField = DialogForm.Show("Порт", Global.username, Global.DialogTypes.edit, out result);
                  if (result == DialogResult.OK)
                  {
                      Global.receivePort = Convert.ToInt32(editField);
                      address.Text = String.Format(Global.externalIP + ":{0}", Global.receivePort);
                      changePortButton.Location = new Point(address.Location.X + address.Width, address.Location.Y);
                      Global.connection = new RemoteConnection();
                  }
              });
            /*По нажатию на кнопку обновляем пароль*/
            refreshPassButton.MouseClick += RefreshPassButton_MouseClick;
            WorkingArea.Controls.Add(changeUsernameButton);
            WorkingArea.Controls.Add(changePortButton);
            WorkingArea.Controls.Add(refreshPassButton);
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
                {
                    server.Show();
                    connectionMonitor.Stop();
                }
                else
                {
                    server.Hide();
                }
            });
            connectionMonitor.Start();
        }

        public override void DBForm_FormClosing(object sender, FormClosingEventArgs e)
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
