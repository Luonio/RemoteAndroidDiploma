using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormTry_1
{
    class RemoteActions
    {
        #region Переменные
        /*Действия, полученные от клиента для реализации на сервере*/
        public Queue<DataSet> serverActions;
        /*Дествия, отправляемые сервером для реализации на клиенте*/
        public Queue<DataSet> clientActions;
        #endregion

        #region Конструкторы
        public RemoteActions()
        {
            this.serverActions = new Queue<DataSet>();
            this.clientActions = new Queue<DataSet>();
            /*Запускаем обработку действий*/
            Task.Run(ExecuteActionsAsync);
        }
        #endregion

        /*Выполнение полученных от клиента команд*/
        public async Task ExecuteActionsAsync()
        {
            while (true)
            {
                if (serverActions.Count != 0)
                {                    
                    DataSet currentAction = serverActions.Dequeue();
                    switch (currentAction.command)
                    {
                        //TODO: сделать переключатель действий для первой команды в очереди
                    }
                }
            }
        }
    }

    public class DataSet
    {
        #region Переменные
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

        /*Преобразует команду в строку*/
        private String ToString(ConnectionCommands command)
        {
            switch (command)
            {
                case ConnectionCommands.INIT:
                    return "0x01\\";
                case ConnectionCommands.PASSWORD:
                    return "0x02\\";
                case ConnectionCommands.CONNECT:
                    return "0x03\\";
                case ConnectionCommands.DECLINE:
                    return "0x04\\";
                case ConnectionCommands.EXIT:
                    return "0x05\\";
                case ConnectionCommands.ERROR:
                    return "0x06\\";
                default:
                    return "0x00\\";
            }
        }
        #endregion
    }
}
