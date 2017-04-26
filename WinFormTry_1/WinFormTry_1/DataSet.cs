using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormTry_1
{
    public class DataSet
    {
        /*Команда*/
        public Global.ConnectionCommands command;

        /*Массив данных*/
        public String[] variables;

        /*Пакет*/
        public String package;

        public DataSet()
        {
            this.command = Global.ConnectionCommands.NONE;
            this.variables = null;
        }

        /*Получаем пакет вида КОМАНДА:данные,данные,данные...*/
        public DataSet(String package)
        {
            variables = package.Split(new Char[] { '\'', ',' });
            this.command = ToCommand(variables[0]);
            this.package = package;
        }

        /*Преобразует строку в команду*/
        private Global.ConnectionCommands ToCommand(String str)
        {
            switch(str)
            {
                /*INIT:remoteUsername,remoteDevice
                 var1 = username
                 var2 = device*/
                case "0x01":
                    return Global.ConnectionCommands.INIT;
                case "0x02":
                    return Global.ConnectionCommands.PASSWORD;
                case "0x03":
                    return Global.ConnectionCommands.CONNECT;
                case "0x04":
                    return Global.ConnectionCommands.DECLINE;
                case "0x05":
                    return Global.ConnectionCommands.EXIT;
                case "0x06":
                    return Global.ConnectionCommands.ERROR;
                default:
                    return Global.ConnectionCommands.NONE;
            }
        }
    }
}
