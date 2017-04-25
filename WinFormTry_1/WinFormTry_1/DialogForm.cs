using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DarkBlueTheme;

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
            mess.Location = new Point(10, Height / 2 - mess.Height / 2);
            mess.ForeColor = Global.itemTextColor;
        }

        /*В зависимости от выбранного типа диалога настраиваем кнопки*/
        private void AddButtons()
        {
            switch (dialogType)
            {
                case Global.DialogTypes.none:
                    break;
                case Global.DialogTypes.close:
                    Controls.Add(NewButton("Да", new Point(10, Height - 30), DialogResult.Yes));
                    Controls.Add(NewButton("Нет", new Point(Width - 85, Height - 30), DialogResult.No));
                    break;
                case Global.DialogTypes.message:
                    Controls.Add(NewButton("ОК", new Point(ClientRectangle.Width - 85, Height - 30), DialogResult.OK));
                    break;
            }
        }

        /*Добавление кнопки с указанным текстом, позицией и результатом диалога*/
        private Button NewButton(string text, Point location, DialogResult res)
        {
            DBButton bt = new DBButton();
            bt.Text = text;
            bt.Location = location;
            bt.FlatStyle = FlatStyle.Flat;
            bt.BackColor = Global.buttonColor;
            /*Обрабатываем нажатие на левую кнопку мыши*/
            bt.MouseClick += new MouseEventHandler((o, mEv) =>
             {
                 if (mEv.Button == MouseButtons.Left)
                 {
                     result = res;
                     this.Close();
                 }
             });
            return bt;
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
