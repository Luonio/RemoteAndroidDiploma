using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormTry_1
{
    public partial class AltActionsControl : MenuItemControl
    {
        /*Активируется при переключении состояний*/
        Bitmap altImg;
        String altText;
        /*Состояние переключаемой иконки
          - для включения/отключения сеанса: true - выключено, false - включено
          - для открытия окон: true - открыто, false - закрыто*/
        bool altChanged = false;

        public AltActionsControl()
        {
            InitializeComponent();
        }

        public AltActionsControl(string startTxt, Bitmap startBmp, String stopTxt, Bitmap stopBmp)
        {
            this.img = startBmp;
            this.text = startTxt;
            altImg = stopBmp;
            altText = stopTxt;
            InitializeComponent();
            /*Задаем стиль контрола*/
            this.iconBox.Image = img;
            
        }

        private void AltActionsControl_Load(object sender, EventArgs e)
        {
            this.ForeColor = Global.itemTextColor;
        }


        /*При нажатии на контрол*/
        private void AltActionsControl_MouseClick(object sender, MouseEventArgs e)
        {
            if(altChanged)
            {
                iconBox.Image = img;
                itemNameBox.Text = text;
            }
            else
            {
                iconBox.Image = altImg;
                itemNameBox.Text = altText;
            }
            altChanged = !altChanged;
        }
    }
}
