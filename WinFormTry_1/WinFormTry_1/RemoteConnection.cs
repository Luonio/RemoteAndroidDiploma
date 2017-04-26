using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace WinFormTry_1
{
    public class RemoteConnection
    {
        /*Имя компьютера*/
        public String device;
        /*ip-адрес компьютера*/
        public IPAddress hostAdress;
        /*Порт, по которому будет проводиться соединение*/
        public int port=-1;
        /*Код доступа*/
        public String securityCode;
        /*Android-клиент, подключающийся к компьютеру*/
        RemoteDevice remoteClient;

        private Socket handler;

        /*Имя пользователя*/
        public String username
        {
            get
            {
                if (Global.username != null)
                    return Global.username;
                else
                    return device;
            }
            set
            {
                Global.username = value;
            }
        }

        /*Сетевая настройка подключения*/
        public IPEndPoint host
        {
            get
            {
                if (this.hostAdress != null & this.port != -1)
                    return new IPEndPoint(this.hostAdress, this.port);
                return null;
            }
            set { }
        }

        /*Конструктор класса*/
        public RemoteConnection()
        {
            this.hostAdress = IPAddress.Parse(Global.hostIP);
            this.port = Global.port;
            this.device = Environment.MachineName;
            this.securityCode = Global.securityCode;
        }

        /*Запуск асинхронного прослушивания подключений*/
        public void Listen()
        {
            Task listen = Task.Factory.StartNew(() =>
            {
                Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                try
                {
                    sListener.Bind(host);
                    sListener.Listen(1);
                    /*Ожидаем входящего подключения*/
                    handler = sListener.Accept();
                    /*Мы дождались клиента, пытающегося с нами соединиться
                      Отправляем ему команду-запрос данных*/
                    handler.Send(Encoding.ASCII.GetBytes(Global.Commands.INIT));                    
                }
                catch (Exception ex)
                {
                    DialogForm.Show("Ошибка", ex.Message, Global.DialogTypes.message);
                }
            });
        }
    }


    /*Подключаемое устройство*/
    public class RemoteDevice
    {
        /*Идентификатор удаленного устройства
          Составляется из сгенерированного пароля, использованного при 
          первом подключении*/
        public String id;
        /*Имя пользователя удаленного устройства*/
        public String username;
        /*Название удаленного устройства*/
        public String device;


        public bool noPasswordAllowed => CheckDevice(this);

        /*Возвращает true, если подключаемое устройство - сохраненное*/
        private static bool CheckDevice(RemoteDevice device)
        {
            /*Если ссылаемый объект проинициализирован*/
            if (device.username != null & device.device != null & device.id != null)
                foreach (RemoteDevice d in Global.savedDevices)
                {
                    if (device.Equals(d))
                        return true;
                }
            return false;                    
        }

        public bool Equals(RemoteDevice dev)
        {
            if (id == dev.id & username == dev.username & device == dev.device)
                return true;
            return false;
        }
    }
}
