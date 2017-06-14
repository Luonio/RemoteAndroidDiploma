using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormTry_1
{
    public class MediaData
    {
        #region Поля
        private Queue<DataSet> sendQueue;
        private Queue<DataSet> receiveQueue;

        /*Проверка, есть ли данные на отправку*/
        public bool Available
        {
            get
            {
                if (sendQueue.Count != 0)
                    return true;
                return false;
            }
        }
        #endregion

        #region Конструкторы
        public MediaData()
        {
            sendQueue = new Queue<DataSet>();
            receiveQueue = new Queue<DataSet>();
        }
        #endregion

        #region Методы

        #region Работа с очередями извне
        /*Получает объект из очереди-получателя*/
        public DataSet Get()
        {
            lock (receiveQueue)
                if (receiveQueue.Count != 0)
                {
                    return receiveQueue.Dequeue();
                }
                else return null;
        }

        /*Вносит объект в очередь-отправитель*/
        public void Put(DataSet data)
        {
            lock (sendQueue)
                sendQueue.Enqueue(data);
        }

        /*Чтение объекта в очередь-получатель*/
        public void PutToReceived(DataSet data)
        {
            lock (receiveQueue)
                receiveQueue.Enqueue(data);
        }

        /*Получение объекта из очереди-отправителя*/
        public DataSet GetFromSending()
        {
            lock (sendQueue)
                if (sendQueue.Count != 0)
                {
                    return sendQueue.Dequeue();
                }
                else return null;
        }
        #endregion

        /*Выполнение полученных от клиента команд*/
        public async Task ExecuteActionsAsync()
        {
            if (receiveQueue.Count != 0)
            {
                DataSet currentAction;
                lock (receiveQueue)
                    currentAction = receiveQueue.Dequeue();
                switch (currentAction.command)
                {
                    case DataSet.ConnectionCommands.CHATMESSAGE:
                        /*Преобразуем структуру в сообщение*/
                        ChatMessage message = new ChatMessage(ChatMessage.MessageType.Incoming, currentAction.variables[1].ToString());
                        Global.chat.ShowMessage(message);
                        break;
                }
            }
        }
        #endregion
    }
}
