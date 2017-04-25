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
    public partial class ChatForm : ChildFormsTemplate
    {
        /*Рабочее пространство*/
        Panel chatPanel;

        /*Контролы для отправки текста*/
        TextBox enterBox;
        Button enterButton;

        private int enterHeight = 30;


        public ChatForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            //Задаем иконку
            this.Icon = Properties.Resources.header_chat;
            #region Добавляем контролы
            /*Создаем панельку и помещаем ее на форму*/
            chatPanel = new Panel();
            SetPanelBounds(ref chatPanel);
            Controls.Add(chatPanel);
            /*Создаем textBox для ввода и помещаем его на форму*/
            enterBox = new DBTextBox();
            enterBox.Multiline = true;
            enterBox.WordWrap = true;
            SetTextBoxBounds(ref enterBox);
            enterBox.KeyDown+= EnterBox_KeyDown;
            chatPanel.Controls.Add(enterBox);
            /*Создаем кнопку для отправки сообщения*/
            enterButton = new Button();
            enterButton.FlatStyle = FlatStyle.Flat;
            enterButton.FlatAppearance.BorderSize = 0;
            enterButton.Image = new Bitmap(Properties.Resources.enter_icon,
                        new Size(enterHeight, enterHeight));
            enterButton.BackColor = Global.textBoxColor;
            SetButtonBounds(ref enterButton);
            chatPanel.Controls.Add(enterButton);
            #endregion
            this.FormClosing -= ChildFormsTemplate_FormClosing;
            this.FormClosing += ChatForm_FormClosing;
        }

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

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
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

        /*Задает позицию и размеры панели для отображения элементов*/
        private void SetPanelBounds(ref Panel panel)
        {
            panel.Height = ClientRectangle.Height-10;
            panel.Width = Convert.ToInt32(ClientRectangle.Width - 20);
            panel.BackColor = Global.addWindowColor;
            panel.Location = new Point(10, ClientRectangle.Y);
        }

        /*Настраиваем TextBox*/
        private void SetTextBoxBounds(ref TextBox box)
        {
            box.Size = new Size(chatPanel.Width-enterHeight, enterHeight);
            box.Location = new Point(0, chatPanel.Height - box.Height-1);
        }

        /*Настраиваем кнопку*/
        private void SetButtonBounds(ref Button but)
        {
            enterButton.Size = new Size(enterHeight, enterHeight);
            enterButton.Location = new Point(chatPanel.Width - enterHeight, 
                chatPanel.Height - but.Height - 1);

        }

        /*При изменении размера формы*/
        private void ChatForm_Resize(object sender, EventArgs e)
        {
            if (ClientRectangle != Rectangle.Empty)
            { 
                SetPanelBounds(ref chatPanel);
                SetTextBoxBounds(ref enterBox);
                SetButtonBounds(ref enterButton);
            }
        }

    }
}
