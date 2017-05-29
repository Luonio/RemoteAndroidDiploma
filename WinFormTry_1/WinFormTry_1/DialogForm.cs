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
    public partial class DialogForm : DBForm
    {
        Global.DialogTypes dialogType;
        DialogResult result;

        /*Если вызывается диалог изменения свойства, сюда будет записываться результат*/
        public string editText;

        public DBLabel message;

        /*Gлучаем заголовок, содержимое и тип вызываемой формы*/
        public DialogForm(String header, String text, Global.DialogTypes dialog)
        {
            InitializeComponent();
            this.Size = new Size(260, 150);
            this.Text = header;
            message = new DBLabel(text);
            editText = text;
            this.dialogType = dialog;
            this.Width = 260;
            this.Height = 150;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void DialogForm_Load(object sender, EventArgs e)
        {
            AddControls();
            /*Настраивать видимость формы так же не нужно*/
            this.VisibleChanged -= DBForm_VisibleChanged;
        }

        /*Настраиваем label*/
        private void SetMessageBounds(DBLabel mess)
        {
            mess.Location = new Point(10, WorkingArea.Height / 2 - mess.Height);
        }

        /*Добавление контролов в зависимости от типа диалога*/
        private void AddControls()
        {
            switch(dialogType)
            {
                case Global.DialogTypes.none:
                    break;
                case Global.DialogTypes.message:
                    SetMessageBounds(message);
                    WorkingArea.Controls.Add(message);
                    WorkingArea.Controls.Add(NewButton("ОК", new Point(WorkingArea.Width - 85, WorkingArea.Height - 30), DialogResult.OK));
                    break;

                case Global.DialogTypes.close:
                    SetMessageBounds(message);
                    WorkingArea.Controls.Add(message);
                    WorkingArea.Controls.Add(NewButton("Да", new Point(10, WorkingArea.Height - 30), DialogResult.Yes));
                    WorkingArea.Controls.Add(NewButton("Нет", new Point(WorkingArea.Width - 85, WorkingArea.Height - 30), DialogResult.No));
                    break;
                case Global.DialogTypes.edit:
                    this.Icon = Properties.Resources.header_properties;
                    WorkingArea.Controls.Add(NewEditBox(editText, new Size(WorkingArea.Width / 2, 20), new Point(15, WorkingArea.Height / 2 - 20)));
                    WorkingArea.Controls.Add(NewButton("Применить", new Point(10, WorkingArea.Height - 30), DialogResult.OK));
                    WorkingArea.Controls.Add(NewButton("Отмена", new Point(WorkingArea.Width - 85, WorkingArea.Height - 30), DialogResult.Cancel));
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

        /*Добавление поля для ввода текста с указанным начальным значением, позицией и размерами*/
        private TextBox NewEditBox(string initText, Size size, Point location)
        {
            DBTextBox editBox = new DBTextBox(initText);
            editBox.Size = size;
            editBox.Location = location;
            editBox.TextChanged += ((o, ev) =>
              {
                  editText = editBox.Text;
              });
            return editBox;
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

        public static string Show(String text, String caption, Global.DialogTypes type, out DialogResult dialogResult)
        {
            DialogForm tempDialog = new DialogForm(text, caption, type);
            tempDialog.ShowDialog();
            dialogResult = tempDialog.result;
            string newValue = tempDialog.editText;
            tempDialog.Close();
            return newValue;
        }
    }

}
