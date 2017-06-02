using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using System.Net.NetworkInformation;

namespace WinFormTry_1
{
    /*Глобальные переменные проекта*/
    public static class Global
    {
        public enum DialogTypes
        {
            none = 0,
            close,
            message,
            edit
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

        private static String hostIP = "192.168.0.101";
        /*Внешний ip роутера*/
        public static IPAddress externalIP
        {
            get
            {
                string ip = new WebClient().DownloadString("http://icanhazip.com");
                if (ip.Contains('\n'))
                    return IPAddress.Parse(ip.Remove(ip.Length - 1, 1));
                return IPAddress.Parse(ip);
            }
        }
        public static int receivePort = 65011;
        public static int sendPort = 65002;
        /*public static int screenPort = 65000 ;
        public static int communicationPort = 65001;*/
        public static String username = Environment.MachineName;
        public static String securityCode;
        public static RemoteConnection connection;
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
            /*StringBuilder result = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < 0x10)
                    result.Append("0");
                result.Append(array[i].ToString("X"));
            }
            return result.ToString();*/
            return Encoding.ASCII.GetString(array, 0, array.Length);
        }

        /*Возвращает true, если картинки идентичны*/
        public static bool Compare (this Bitmap bmp, Bitmap anotherBmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms,ImageFormat.Jpeg);
            byte[] bt1 = ms.GetBuffer();
            anotherBmp.Save(ms, ImageFormat.Jpeg);
            byte[] bt2 = ms.GetBuffer();
            if (bt1.Length != bt2.Length)
                return true;
            for (int i = 0; i < bt1.Length; i++)
                if (bt1[i] != bt2[i])
                    return true;
            return true;
        }

        /*Отмечаем элемент для пересылки*/
        public static void Check(this List<ScreenPart> list, int number)
        {
            list.FindPart(number).changed = true;
        }

        /*Находим в списке элемент с указанным номером*/
        public static ScreenPart FindPart(this List<ScreenPart> list, int number)
        {
            foreach (ScreenPart part in list)
                if (part.partNumber == number)
                    return part;
            return null;
        }
        #endregion
    }
}
