using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WinFormTry_1
{
    /*Своя версия диалогового окна с подтверждением*/
    public partial class DialogForm : ChildFormsTemplate
    {
        Global.DialogTypes dialogType;
        DialogResult result;

        public Label message;

        /*Gлучаем заголовок, содержимое и тип вызываемой формы*/
        public DialogForm(String header, String text, Global.DialogTypes dialog)
        {
            InitializeComponent();
            this.Text = header;
            message = new Label();
            this.message.Text = text;
            this.dialogType = dialog;
            this.Width = 260;
            this.Height = 150;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void DialogForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.notify_icon;
            /*Настраивать видимость формы так же не нужно*/
            this.VisibleChanged -= ChildFormsTemplate_VisibleChanged;
            /*Добавляем контролы*/
            SetMessageBounds(message);
            Controls.Add(message);
            AddButtons();
        }

        /*Настраиваем label*/
        private void SetMessageBounds(Label mess)
        {
            mess.AutoSize = true;
            mess.Location = new Point(10, Height / 2 - mess.Height/2);
            mess.ForeColor = Global.itemTextColor;
        }

        /*В зависимости от выбранного типа диалога настраиваем кнопки*/
        private void AddButtons()
        {
            switch(dialogType)
            {
                case Global.DialogTypes.none:
                    break;
                case Global.DialogTypes.close:
                    AddYesButton();
                    AddNoButton();
                    break;
            }
        }

        /*Добавление кнопки "да" и обработчика нажатия на нее*/
        private void AddYesButton()
        {
            Button yesButton = new Button();
            yesButton.Text = "Да";
            yesButton.Location = new Point(10, Height-yesButton.Height-10);
            yesButton.FlatStyle = FlatStyle.Flat;
            yesButton.BackColor = Global.buttonColor;
            /*Обрабатываем нажатие на левую кнопку мыши*/
            yesButton.MouseClick += new MouseEventHandler((o, mEv) =>
            {
                if (mEv.Button == MouseButtons.Left)
                {
                    result = DialogResult.Yes;
                    this.Close();
                }
            });
            Controls.Add(yesButton);
            this.AcceptButton = yesButton;
        }

        /*Добавление кнопки "нет" и обработчика нажатия на нее*/
        private void AddNoButton()
        {
            Button noButton = new Button();
            noButton.Text = "Нет";
            noButton.Location = new Point(Width - noButton.Width - 10, Height - noButton.Height - 10);
            noButton.FlatStyle = FlatStyle.Flat;
            noButton.BackColor = Global.buttonColor;
            /*Обрабатываем нажатие на левую кнопку мыши*/
            noButton.MouseClick += new MouseEventHandler((o, mEv) =>
            {
                if (mEv.Button == MouseButtons.Left)
                {
                    result = DialogResult.No;
                    this.Close();
                }
            });
            Controls.Add(noButton);
        }

        /*Модальное отображение диалога*/
        public static DialogResult Show(String text, String caption, Global.DialogTypes type )
        {
            DialogForm tempDialog = new DialogForm(text, caption, type);
            tempDialog.ShowDialog();
            DialogResult dialog = tempDialog.result;
            tempDialog.Close();
            return dialog;
        }
    }

}
