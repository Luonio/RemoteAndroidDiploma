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

        private Socket remoteListener;

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

            /*Инициализируем сокет*/
            remoteListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            /*Запуск асинхронного прослушивания сокета*/
            Task listeningTask = new Task(Listen);
            listeningTask.Start();
        }

        /*Поток для приема подключений*/
        public void Listen()
        {
            try
            {
                /*Привязываем сокет к серверному адресу*/
                remoteListener.Bind(host);
                /*Считываем начальные данные об удаленном устройстве*/
                while (true)
                {
                    /*Получаем сообщение*/
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    /*Адрес, с которого приходят данные*/
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, port);
                    /*Получаем данные и преобразуем их в DataSet*/
                    bytes = remoteListener.ReceiveFrom(data, ref remoteIp);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    DataSet initStructure = new DataSet(builder.ToString());
                    /*Проверяем операцию
                      Как только отловили команду INIT, инициализируем удаленного пользователя*/
                    if (initStructure.command==DataSet.ConnectionCommands.INIT)
                    {
                        /*Получаем ip, с которого пришел сигнал*/
                        IPEndPoint finalIp = remoteIp as IPEndPoint;
                        remoteClient = new RemoteDevice(initStructure.variables[0], initStructure.variables[1], finalIp.Address);
                        break;
                    }
                }
            }
            catch { }
        }

        /*Отправка данных*/
        public void Send(DataSet package)
        {
            byte[] data = Encoding.Unicode.GetBytes(package.package);
            remoteListener.SendTo(data, host);
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
        /*IP удаленного устройства*/
        IPAddress ip;

        public bool noPasswordAllowed => CheckDevice(this);

        /*Конструктор*/
        public RemoteDevice(string username, string deviceName, IPAddress ip )
        {
            this.username = username;
            this.device = deviceName;
            this.ip = ip;
        }

        /*Возвращает true, если подключаемое устройство - сохраненное*/
        private static bool CheckDevice(RemoteDevice device)
        {
            /*Если ссылаемый объект проинициализирован*/
            if (device.username != null & device.device != null)
                foreach (RemoteDevice d in Global.savedDevices)
                {
                    if (device.Equals(d))
                        return true;
                }
            return false;                    
        }

        public bool Equals(RemoteDevice dev)
        {
            if (username == dev.username & device == dev.device)
                return true;
            return false;
        }
    }
}
