using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using Open.Nat;
using System.Threading;


namespace WinFormTry_1
{
    public class RemoteConnection
    {
        /*Список сохраненных девайсов
          с устройств из этого списка можно подключаться без пароля*/
        public static List<RemoteDevice> savedDevices = new List<RemoteDevice>();

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

        /*Уровень доступа клиента.
          None - ожидание команды INIT
          SendPassword - получение пароля
          Connect - отправка команды Connect и получение данных инициализации сервера
          SendData - обмен данными с сервером*/
        private enum AccessLevel
        {
            None,
            SendPassword,
            Connect, 
            SendData
        }

        AccessLevel access = AccessLevel.None;

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

        public IPEndPoint client;

        /*Внешний ip роутера*/
        public IPAddress externalIP
        {
            get
            {
                string ip = new WebClient().DownloadString("http://icanhazip.com");
                if (ip.Contains('\n'))
                    return IPAddress.Parse(ip.Remove(ip.Length - 1, 1));
                return IPAddress.Parse(ip);
            }
        }

        /*Конструктор класса*/
        public RemoteConnection()
        {
            this.hostAdress = IPAddress.Parse(Global.hostIP);
            this.port = Global.receivePort;
            this.device = Environment.MachineName;
            this.securityCode = Global.securityCode;

            /*Инициализируем сокет*/
            remoteListener = new Socket(host.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            Task.Run(ListenAsync);
        }

        /*Поток для приема подключений*/
        public async Task ListenAsync()
        {
            try
            {
                var nat = new NatDiscoverer();
                var cts = new CancellationTokenSource(5000);
                // using SSDP protocol, it discovers NAT device.
                var device = await nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                // create a new mapping in the router [external_ip:1702 -> host_machine:1602]
                await device.GetExternalIPAsync();
                await device.CreatePortMapAsync(new Mapping(Protocol.Udp, port, port, "Server"));

                /*Привязываем сокет к серверному адресу*/
                remoteListener.Bind(host);
                /*Ждем инициализации удаленного устройства*/
                await Connect();
            }
            catch (Exception ex)
            {
                DialogForm.Show("Ошибка", ex.ToString(), Global.DialogTypes.message);
            }
               
        }

        /*Выполняет все этапы подключения клиента к серверу*/
        private async Task Connect ()
        {
            while (true)
            {
                switch (access)
                {
                    #region INIT
                    /*Ждем сообщения INIT от клиента*/
                    case AccessLevel.None:
                        /*Считываем начальные данные об удаленном устройстве*/
                        EndPoint remoteIp = new IPEndPoint(IPAddress.Any, port);
                        /*Получаем сообщение*/
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; // количество полученных байтов
                        byte[] data = new byte[256]; // буфер для получаемых данных
                                                     /*Получаем данные и преобразуем их в DataSet*/
                        bytes = remoteListener.ReceiveFrom(data, ref remoteIp);
                        builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
                        DataSet initStructure = new DataSet(builder.ToString());
                        /*Проверяем операцию
                          Как только отловили команду INIT, инициализируем удаленного пользователя*/
                        if (initStructure.command == DataSet.ConnectionCommands.INIT)
                        {
                            /*Получаем ip, с которого пришел сигнал*/
                            client = remoteIp as IPEndPoint;
                            client.Port = Global.receivePort;
                            remoteClient = new RemoteDevice(initStructure.variables[0], initStructure.variables[1], client.Address);
                            access = AccessLevel.SendPassword;
                        }
                        else
                            access = AccessLevel.None;
                        break;
                    #endregion
                    #region PASSWORD
                    case AccessLevel.SendPassword:
                        /*Проверяем наличие удаленного устройства в сохраненных*/
                        if (!savedDevices.Contains(remoteClient))
                        {
                            /*Устройства в сохраненных нет. Запрашиваем пароль*/
                            DataSet passStructure = new DataSet(DataSet.ConnectionCommands.PASSWORD);
                            Send(passStructure);
                            /*Ждем ответа*/
                            passStructure = ReadPackage(client);
                            /*Проверяем операцию
                                Как только отловили команду PASSWORD, проверяем указанный в сообщении пароль*/
                            if (passStructure.command == DataSet.ConnectionCommands.PASSWORD)
                            {
                                if (passStructure.variables[0] != securityCode)
                                    access = AccessLevel.None;
                                else
                                    access = AccessLevel.Connect;
                            }
                            else if (passStructure.command == DataSet.ConnectionCommands.EXIT)
                            {
                                access = AccessLevel.None;
                            }
                        }
                        /*Устройство есть в сохраненных. Подключаемся*/
                        else
                            access = AccessLevel.Connect;
                        break;
                    #endregion
                    case AccessLevel.Connect:
                        access = AccessLevel.SendData;
                        break;
                    /*Если получили доступ к передаче данных, выходим из метода инициализации*/
                    case AccessLevel.SendData:
                        return;
                }
            }
        }

        /*Чтение одного пакета*/
        private DataSet ReadPackage (EndPoint point)
        {
            /*Получаем сообщение*/
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных
            /*Получаем данные и преобразуем их в DataSet*/
            bytes = remoteListener.ReceiveFrom(data, ref point);
            builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
            return new DataSet(builder.ToString());
        }

        /*Отправка данных*/
        public void Send(DataSet package)
        {
            byte[] data = package.ToByteArray();
            remoteListener.SendTo(data, client);
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
                foreach (RemoteDevice d in RemoteConnection.savedDevices)
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
