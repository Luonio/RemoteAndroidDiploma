using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormTry_1
{
    public class DataSet
    {
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

        /*Команда*/
        public ConnectionCommands command;

        /*Массив данных*/
        public String[] variables;

        /*Пакет*/
        public String package;

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

        /*Делим строку на команду и массив данных*/
        private void FromString(String package)
        {
            String[] tmpArr = package.Split('\\');
            this.command = ToCommand(tmpArr[0]);
            if (tmpArr[1] != null)
                variables = tmpArr[1].Split(',');
            this.package = package;
        }

        /*Преобразует строку в команду*/
        private ConnectionCommands ToCommand(String str)
        {
            switch(str)
            {
                /*0x01:remoteUsername,remoteDevice
                 var0 = username
                 var1 = device*/
                case "0x01":
                    return ConnectionCommands.INIT;
                case "0x02":
                    return ConnectionCommands.PASSWORD;
                case "0x03":
                    return ConnectionCommands.CONNECT;
                case "0x04":
                    return ConnectionCommands.DECLINE;
                case "0x05":
                    return ConnectionCommands.EXIT;
                case "0x06":
                    return ConnectionCommands.ERROR;
                default:
                    return ConnectionCommands.NONE;
            }
        }
    }
}
