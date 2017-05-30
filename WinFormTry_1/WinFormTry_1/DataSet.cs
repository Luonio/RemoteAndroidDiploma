using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormTry_1
{
    public class DataSet
    {
        #region Переменные
        public enum ConnectionCommands
        {
            NONE = 0x00,
            HELLO = 0x01,
            INIT = 0x02,
            PASSWORD = 0x03,
            CONNECT = 0x04,
            DECLINE = 0x05,
            EXIT = 0x06,
            ERROR = 0x07,
            SCREEN = 0x08,
            SCREENINFO = 0x09
        }

        /*Команда*/
        public ConnectionCommands command;

        /*Массив данных*/
        public List<String> variables = new List<String>();

        /*Пакет*/
        public String package;
        #endregion

        #region Конструкторы
        public DataSet()
        {
            this.command = ConnectionCommands.NONE;
            this.variables = null;
        }

        /*Получаем пакет вида КОМАНДА\данные,данные,данные...*/
        public DataSet(String package)
        {
            FromString(package);
        }

        /*Получаем пакет в виде массива байтов*/
        public DataSet(Byte[] package)
        {
            FromString(package.ToString());
        }

        /*Получаем команду для инициализации пакета*/
        public DataSet(ConnectionCommands command)
        {
            this.command = command;
            this.package = ToString(command);
        }
        #endregion

        #region Методы
        /*Добавление переменной в variables*/
        public void Add(Object data)
        {
            if (variables.Count() == 0)
                package += data.ToString();
            else
                package += (String.Format("," + data.ToString()));
            variables.Add(data.ToString());

        }

        /*Преобразует набор данных в массив байтов*/
        public Byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(package);
        }

        /*Делим строку на команду и массив данных*/
        private void FromString(String package)
        {
            String[] tmpArr = package.Split('\\');
            this.command = ToCommand(tmpArr[0]);
            if (tmpArr[1] != "")
                foreach (String value in tmpArr[1].Split(','))
                    variables.Add(value);
            this.package = package;
        }

        /*Преобразует строку в команду*/
        private ConnectionCommands ToCommand(String str)
        {
            switch (str)
            {
                case "0x01":
                    return ConnectionCommands.HELLO;
                /*0x01:remoteUsername,remoteDevice
                 var0 = username
                 var1 = device*/
                case "0x02":
                    return ConnectionCommands.INIT;
                case "0x03":
                    return ConnectionCommands.PASSWORD;
                case "0x04":
                    return ConnectionCommands.CONNECT;
                case "0x05":
                    return ConnectionCommands.DECLINE;
                case "0x06":
                    return ConnectionCommands.EXIT;
                case "0x07":
                    return ConnectionCommands.ERROR;
                case "0x08":
                    return ConnectionCommands.SCREEN;
                case "0x09":
                    return ConnectionCommands.SCREENINFO;
                default:
                    return ConnectionCommands.NONE;
            }
        }

        /*Преобразует команду в строку*/
        private String ToString(ConnectionCommands command)
        {
            switch (command)
            {
                case ConnectionCommands.HELLO:
                    return "0x01\\";
                case ConnectionCommands.INIT:
                    return "0x02\\";
                case ConnectionCommands.PASSWORD:
                    return "0x03\\";
                case ConnectionCommands.CONNECT:
                    return "0x04\\";
                case ConnectionCommands.DECLINE:
                    return "0x05\\";
                case ConnectionCommands.EXIT:
                    return "0x06\\";
                case ConnectionCommands.ERROR:
                    return "0x07\\";
                case ConnectionCommands.SCREEN:
                    return "0x08\\";
                case ConnectionCommands.SCREENINFO:
                    return "0x09\\";
                default:
                    return "0x00\\";
            }
        }
        #endregion
    }
}
