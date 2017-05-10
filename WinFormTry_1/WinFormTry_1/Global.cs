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
            message = 2
        }

        #region Поля
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
        public static Color textBoxColor = Color.FromArgb(200, 200, 200);

        public static String hostIP = "192.168.0.105";
        public static int screenPort = 65000;
        public static int communicationPort = 65001;
        public static String username;
        public static String securityCode;
        public static ScreenActions screenActions;

        /*Интервал между отправками данных клиенту*/
        public static int connectionInterval = 40;
        #endregion

        #region Методы расширения
        /*Преобразует массив байтов в строку вида "...число_число_число_число..."*/
        public static String GetString(this Byte[] array)
        {
            /*Если передан null или пустой массив, возвращаем null*/
            if (array == null | array.Length == 0)
                return null;
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                result.Append(array[i].ToString("X"));
                result.Append('_');
            }
            return result.ToString();
        }
        #endregion
    }
}
