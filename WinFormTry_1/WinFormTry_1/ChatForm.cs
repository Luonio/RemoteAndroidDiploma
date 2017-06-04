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
using System.Runtime.InteropServices;

namespace WinFormTry_1
{
    public partial class ChatForm : DBForm
    {
        #region Поля

        public Queue<ChatMessage> sendMessageQueue;
        public Queue<ChatMessage> receiveMessaageQueue;

        /*Контролы для отправки текста*/
        TextBox enterBox;
        Button enterButton;

        /*Сюда будут выводиться сообщения*/
        DBMessagingBox chatTable;

        //Номер строки для вывода сообщения
        private int currentRow = 0;

        private int enterHeight = 30;
        #endregion

        #region Конструкторы
        public ChatForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
            sendMessageQueue = new Queue<ChatMessage>();
            receiveMessaageQueue = new Queue<ChatMessage>();
        }
        #endregion

        #region Методы

        #region События формы
        //Загрузка формы
        private void ChatForm_Load(object sender, EventArgs e)
        {
            //Задаем иконку
            this.Icon = Properties.Resources.header_chat;
            #region Добавляем контролы
            /*Создаем textBox для ввода и помещаем его на форму*/
            enterBox = new DBTextBox();
            enterBox.Multiline = true;
            enterBox.WordWrap = true;
            SetTextBoxBounds(ref enterBox);
            enterBox.KeyDown += EnterBox_KeyDown;
            WorkingArea.Controls.Add(enterBox);
            /*Создаем кнопку для отправки сообщения*/
            enterButton = new Button();
            enterButton.FlatStyle = FlatStyle.Flat;
            enterButton.FlatAppearance.BorderSize = 0;
            enterButton.Image = new Bitmap(Properties.Resources.enter_icon,
                        new Size(enterHeight, enterHeight));
            enterButton.BackColor = Global.textBoxColor;
            SetButtonBounds(ref enterButton);
            WorkingArea.Controls.Add(enterButton);
            chatTable = new DBMessagingBox();
            SetchatTableBounds(ref chatTable);
            WorkingArea.Controls.Add(chatTable);
            enterButton.Click += EnterButton_Click;
        }

        //Нажатие на кнопку "отправить"
        private void EnterButton_Click(object sender, EventArgs e)
        {
            /*Если текстовое поле пустое, ничего не делаем*/
            if (enterBox.Text == "")
                return;
            ChatMessage message = new ChatMessage(enterBox.Text);
            ShowMessage(message);
            enterBox.Text = "";
        }

        #endregion

        //Закрытие формы
        public override void DBForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (enterBox.Text != "")
            {
                DialogResult hideDialog = DialogForm.Show("Выход из чата", "У вас остались неотправленные сообщения.\n Продолжить?", Global.DialogTypes.close);
                if (hideDialog == DialogResult.Yes)
                {
                    enterBox.Clear();
                    this.Hide();
                }
            }
            else
                this.Hide();
        }

        /*Изменение размера формы*/
        private void ChatForm_Resize(object sender, EventArgs e)
        {
            if (WorkingArea != null)
            {
                if (enterBox != null)
                    SetTextBoxBounds(ref enterBox);
                if (enterButton != null)
                    SetButtonBounds(ref enterButton);
                if (chatTable != null)
                    SetchatTableBounds(ref chatTable);
            }
        }
        #endregion

        #region События элементов
        //Клавиша Enter должна срабатывать как кнопка отправки сообщения
        private void EnterBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (e.Shift)
                {
                    enterBox.Text += "\r\n";
                    enterBox.SelectionStart = enterBox.TextLength;
                }   
                else
                    enterButton.PerformClick();
            }
        }

        #endregion

        #region Остальные методы

        #region Управление размерами и положением элементов
        /*Задает позицию и размеры панели для отображения элементов*/
        private void SetchatTableBounds(ref DBMessagingBox panel)
        {
            panel.Size = new Size(Convert.ToInt32(WorkingArea.Width - 5),
                WorkingArea.Height - enterBox.Height - 10);
            panel.Location = new Point(5, 5);
            foreach (MessagePanel box in chatTable.Controls)
            {
                if (box.Message.type == ChatMessage.MessageType.System)
                    box.SetWidth(Convert.ToInt32(chatTable.ColumnStyles[0].Width) * 2);
                else
                {
                    box.SetWidth(Convert.ToInt32(chatTable.ColumnStyles[0].Width));
                    if(box.Message.type==ChatMessage.MessageType.Incoming)
                        box.Location = new Point(0, Convert.ToInt32(chatTable.ColumnStyles[0].Width) - box.Width);
                }
            }
        }

        /*Настраиваем TextBox*/
        private void SetTextBoxBounds(ref TextBox box)
        {
            box.Size = new Size(WorkingArea.Width - enterHeight, enterHeight);
            box.Location = new Point(0, WorkingArea.Height - box.Height - 1);
        }

        /*Настраиваем кнопку*/
        private void SetButtonBounds(ref Button but)
        {
            enterButton.Size = new Size(enterHeight, enterHeight);
            enterButton.Location = new Point(WorkingArea.Width - enterHeight,
                WorkingArea.Height - but.Height - 1);

        }

        #endregion

        //Вывод сообщения на экран
        public void ShowMessage(ChatMessage message)
        {
            MessagePanel mess = new MessagePanel(message);
            switch(message.type)
            {
                case ChatMessage.MessageType.Incoming:
                    mess.SetWidth(Convert.ToInt32(chatTable.ColumnStyles[0].Width));
                    mess.Location = new Point(0, Convert.ToInt32(chatTable.ColumnStyles[0].Width) - mess.Width);
                    chatTable.Controls.Add(mess, 1, currentRow);                    
                    break;
                case ChatMessage.MessageType.Outcoming:
                    mess.SetWidth(Convert.ToInt32(chatTable.ColumnStyles[0].Width));
                    chatTable.Controls.Add(mess, 0, currentRow);
                    break;
                case ChatMessage.MessageType.System:
                    mess.SetWidth(Convert.ToInt32(chatTable.ColumnStyles[0].Width)*2);
                    chatTable.Controls.Add(mess, 0, currentRow);
                    chatTable.SetColumnSpan(mess, 2);
                    break;
            }
            chatTable.ScrollControlIntoView(mess);
            currentRow++;
        }
        #endregion

        #endregion

    }

    //Сообщение
    public class ChatMessage
    {
        #region Поля и структуры
        /*Тип сообщения: входящее или исходящее*/
        public enum MessageType
        {
            //Входящее. Отображается справа
            Incoming = 0,
            //Исходящее. Отображается слева
            Outcoming,
            //Системное. Отображается по центру экрана
            System
        }

        //Имя пользователя, отправившего сообщение
        public string user;
        //Дата отправки
        public DateTime date;
        //Текст сообщения
        public string text;
        //Тип сообщения
        public MessageType type;
        #endregion

        #region Конструкторы
        /*По-умолчанию создается исходящее сообщение с текущим именем пользователя,
         временем, датой и заданным текстом*/
        public ChatMessage(string text)
        {
            user = Global.username;
            type = MessageType.Outcoming;
            date = DateTime.Now;
            this.text = text;
        }

        /*Сообщение, тип которого задается.
         Используется для инициализации объектов под входящие сообщения*/
        public ChatMessage(MessageType type)
        {
            this.type = type;
            date = DateTime.Now;
            text = "";
            if (type == MessageType.System)
                user = "System";
        }

        /*Сообщение, которому передаются тип и текст.
         Используется для вывода системных сообщений*/
        public ChatMessage (MessageType type, string text)
        {
            this.type = type;
            date = DateTime.Now;
            this.text = text;
            if (type == MessageType.System)
                user = "System";
        }
        #endregion   
    }

    public partial class MessagePanel : Panel
    {
        public int maxWidth;
        /*Сообщение, которое будет выведено*/
        public ChatMessage Message;
        /*Контрол для вывода текста*/
        public ChatMessageBox messageBox;
        public MessagePanel(ChatMessage mess)
        {
            this.Message = mess;
            /*Исходя из типа сообщения задаем разные параметры*/
            switch (Message.type)
            {
                case ChatMessage.MessageType.Incoming:
                    this.BackColor = Global.incomingMessageBackColor;
                    break;
                case ChatMessage.MessageType.Outcoming:
                    this.BackColor = Global.outcomingMessageBackColor;
                    break;
                case ChatMessage.MessageType.System:
                    this.BackColor = Global.addWindowColor;
                    break;
            }
            this.BorderStyle = BorderStyle.None;
            messageBox = new ChatMessageBox(Message);
            Height = (TextRenderer.MeasureText(Message.text, messageBox.Font).Width / Width) * Font.Height + 6;
            messageBox.Size = new Size(this.Width - 6, this.Height - 6);
            messageBox.Location = new Point(3, 3);
            messageBox.TextChanged += MessageBox_TextChanged;
            Controls.Add(messageBox);
            this.Resize += MessagePanel_Resize;
        }

        private void MessagePanel_Resize(object sender, EventArgs e)
        {
            messageBox.Size = new Size(this.Width, this.Height);
        }

        private void MessageBox_TextChanged(object sender, EventArgs e)
        {
            Height = TextRenderer.MeasureText(Message.text, messageBox.Font).Width/Width * Font.Height;
        }

        /*Установка максимальной допустимой ширины контрола*/
        public void SetWidth(int width)
        {
            int textWidth = TextRenderer.MeasureText(Message.text, messageBox.Font).Width;
            if (textWidth > width)
                this.Width = width;
            else
                this.Width = textWidth;
            int height = 0;
            /*Для каждой строки сообщения высчитываем высоту*/
            foreach (string txt in Message.text.Split('\n'))
            {
                float wHeight = 0;
                wHeight = (float)TextRenderer.MeasureText(txt, messageBox.Font).Width / (float)Width;
                wHeight = wHeight <= 1 ? wHeight : wHeight + 1;
                height += Convert.ToInt32(Math.Truncate(wHeight));
            }
            Height = height * Font.Height + 6;
        }
    }

    public partial class ChatMessageBox : TextBox
    {
        #region Поля
        /*Сообщение, которое будет выведено*/
        public ChatMessage Message;

        public int maxWidth;
        #endregion

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        #region Конструкторы
        public ChatMessageBox(ChatMessage message)
        {
            this.Message = message;
            this.ReadOnly = true;
            this.Multiline = true;
            this.WordWrap = true;
            this.Margin = new Padding(5);
            this.ForeColor = Global.messageForeColor;
            
            /*Исходя из типа сообщения задаем разные параметры*/
            switch (Message.type)
            {
                case ChatMessage.MessageType.Incoming:
                    this.BackColor = Global.incomingMessageBackColor;
                    this.Text = message.text;
                    break;
                case ChatMessage.MessageType.Outcoming:
                    this.BackColor = Global.outcomingMessageBackColor;
                    this.Text = message.text;
                    break;
                case ChatMessage.MessageType.System:
                    this.BackColor = Global.addWindowColor;
                    this.ForeColor = Color.Red;
                    this.Text = "System: " + Message.text;
                    break;
            }
            this.BorderStyle = BorderStyle.None;
            this.Cursor = Cursors.Arrow;
            this.GotFocus += ChatMessageBox_GotFocus;
        }

        #endregion

        #region Методы

        #region События
        //Убираем каретку, когда сообщение находится в фокусе
        private void ChatMessageBox_GotFocus(object sender, EventArgs e)
        {
            HideCaret(this.Handle);
        }
        #endregion

        #region Остальные методы
        /*Скрываем курсор, так как нам нужно только вывести данные*/
        public void HideCaret()
        {
            HideCaret(this.Handle);
        }

        /*Установка максимальной допустимой ширины контрола*/
        public void SetMaxWidth(int width)
        {
            int textWidth = TextRenderer.MeasureText(Text, this.Font).Width;
            this.maxWidth = width;
            if (textWidth > maxWidth)
                this.Width = maxWidth;
            else
                this.Width = textWidth;
            Height = (Text.Split('\n').Length + 2) * Font.Height;
        }
        #endregion

        #endregion
    }
}
