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

        public enum ConnectionCommands
        {
            NONE = 0x00,
            INIT = 0x01,
            PASSWORD = 0x02,
            CONNECT = 0x03,
            DECLINE = 0x04,
            EXIT = 0x05,
            ERROR = 0x06
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


        public static String hostIP = "127.0.0.1";
        public static int port = 745;
        public static String username;
        public static String securityCode;

        /*Список сохраненных девайсов
          с устройств из этого списка можно подключаться без пароля*/
        public static List<RemoteDevice> savedDevices;

    }
}
