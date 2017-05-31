using System;
using System.Collections.Generic;
using System.IO;
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
        public List<Object> variables = new List<Object>();

        /*Пакет*/
        public MemoryStream package;
        #endregion

        #region Конструкторы
        public DataSet()
        {
            this.command = ConnectionCommands.NONE;
            this.package = new MemoryStream();
        }

        /*Получаем пакет в виде массива байтов*/
        public DataSet(Byte[] pack, int length)
        {
            this.package = new MemoryStream();
            this.package.Write(pack, 0, length);
            /*Устанавливаем позицию в 0 для чтения*/
            this.package.Position = 0;
            /*Читаем команду*/
            byte[] cmdByte = new byte[5];
            this.package.Read(cmdByte, 0, 5);
            this.command = ToCommand(cmdByte);
            /*В зависимости от полученной команды читаем значения переменных*/
            switch (command)
            {
                case ConnectionCommands.INIT:
                    FromString(Encoding.ASCII.GetString(pack, 0, length));
                    break;
                case ConnectionCommands.PASSWORD:
                    int passLength = length - 5;
                    cmdByte = new byte[passLength];
                    this.package.Read(cmdByte, 0, passLength);
                    Add(Encoding.ASCII.GetString(cmdByte, 0, passLength));
                    break;
                case ConnectionCommands.SCREEN:
                    int numLength = 2;
                    cmdByte = new Byte[numLength];
                    this.package.Read(cmdByte, 0, numLength);
                    Add(BitConverter.ToInt16(cmdByte, 0));
                    break;
            }
        }

        /*Получаем команду для инициализации пакета*/
        public DataSet(ConnectionCommands command)
        {
            this.command = command;
            byte[] cmd = Encoding.UTF8.GetBytes(ToString(command));
            package = new MemoryStream();
            package.Write(cmd, 0, cmd.Length);
        }
        #endregion

        #region Методы

        #region Добавление переменной в список
        /*В список переменных добавляется 4хбайтовое знаковое число*/
        public void Add(int data)
        {
            byte[] res = BitConverter.GetBytes(data);
            if(variables.Count!=0)
                package.Write(new byte[] { 44 }, 0, 1);
            package.Write(res, 0, res.Length);
            variables.Add(data);
        }

        /*В список переменных добавляется 2хбайтовое знаковое число*/
        public void Add(short data)
        {
            byte[] res = BitConverter.GetBytes(data);
            if (variables.Count != 0)
                package.Write(new byte[] { 44 }, 0, 1);
            package.Write(res, 0, res.Length);
            variables.Add(data);
        }

        /*В список переменных добавляется строка*/
        public void Add(string data)
        {
            byte[] res = Encoding.ASCII.GetBytes(data);
            if (variables.Count != 0)
                package.Write(new byte[] { 44 }, 0, 1);
            package.Write(res, 0, res.Length);
            variables.Add(data);
        }

        /*В список переменных добавляется массив байтов*/
        public void Add(byte[] data)
        {
            if (variables.Count != 0)
                package.Write(new byte[] { 44 }, 0, 1);
            package.Write(data, 0, data.Length);
            variables.Add(data);
        }
        #endregion

        /*Преобразует набор данных в массив байтов*/
        public Byte[] ToByteArray()
        {
            return package.GetBuffer();
        }

        /*Делим строку на команду и массив данных*/
        private void FromString(String package)
        {
            String[] tmpArr = package.Split('\\');
            if (tmpArr[1] != "")
                foreach (String value in tmpArr[1].Split(','))
                    variables.Add(value);
        }


        /*Преобразует строку в команду*/
        private ConnectionCommands ToCommand(byte[] cmd)
        {
            string str = Encoding.UTF8.GetString(cmd);
            switch (str)
            {
                case "0x01\\":
                    return ConnectionCommands.HELLO;
                /*0x01:remoteUsername,remoteDevice
                 var0 = username
                 var1 = device*/
                case "0x02\\":
                    return ConnectionCommands.INIT;
                case "0x03\\":
                    return ConnectionCommands.PASSWORD;
                case "0x04\\":
                    return ConnectionCommands.CONNECT;
                case "0x05\\":
                    return ConnectionCommands.DECLINE;
                case "0x06\\":
                    return ConnectionCommands.EXIT;
                case "0x07\\":
                    return ConnectionCommands.ERROR;
                case "0x08\\":
                    return ConnectionCommands.SCREEN;
                case "0x09\\":
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
