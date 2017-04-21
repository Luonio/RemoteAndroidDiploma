using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WinFormTry_1
{
    /*Глобальные переменные проекта*/
    public static class Global
    {
        public enum DialogTypes
        {
            none = 0,
            close = 1,
            warning = 2
        }

        /*Цвета форм и составных контролов*/
        public static Color baseWindowColor = Color.FromArgb(0, 10, 30);
        public static Color addWindowColor = Color.FromArgb(30, 180, 200, 220);
        public static Color menuItemColor = Color.Transparent;
        public static Color formBorderColor = Color.FromArgb(180, 200, 220);
        public static Color selectedItemColor = Color.FromArgb(110, 120, 150);
        public static Color clickedItemColor = Color.FromArgb(150, 170, 190);
        /*Кнопки и текст*/
        public static Color selectedItemTextColor = baseWindowColor;
        public static Color clickedItemTextColor = Color.FromArgb(0, 0, 20);
        public static Color itemTextColor = Color.FromArgb(230, 255, 255);
        public static Color buttonColor = Color.FromArgb(150, 160, 190);
        public static Color textBoxColor = Color.FromArgb(200, 200, 200);

    }
}
