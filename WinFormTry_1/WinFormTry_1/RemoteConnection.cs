using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Open.Nat;
using System.Threading;


namespace WinFormTry_1
{
    public class RemoteConnection
    {
        #region Поля
        /*Список сохраненных девайсов
          с устройств из этого списка можно подключаться без пароля*/
        public static List<RemoteDevice> savedDevices = new List<RemoteDevice>();

        /*Текущее состояние подключения*/
        private bool connected = false;

        /*Имя компьютера*/
        public String device;

        /*Действия сервера и клиента*/
        ScreenActions screenActions = Global.screenActions;

        IPAddress clientIP;
        /*Сокет для чтения данных экрана*/
        private Socket screenListener;
        /*Сокет для чтения данных чата и звука*/
        private Socket mediaListener;
        /*Сокет для отправки данных чата и звука*/
        private Socket mediaSender;
        /*Сокет для отправки данных экрана*/
        private Socket screenSender;

        RemoteDevice remoteClient;

        #endregion

        #region Составные типы
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
        #endregion

        #region Свойства
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

        /*Получение данных*/
        public IPEndPoint hostPoint
        {
            get
            {
                return new IPEndPoint(IPAddress.Parse("0.0.0.0"), Global.receivePort);
            }
            set { }
        }

        public IPEndPoint clientPoint
        {
            get
            {
                if(clientIP!=null)
                    return new IPEndPoint(clientIP, Global.sendPort);
                return null;
            }
            set
            {
                clientIP = value.Address;
            }
        }

        public bool Connected
        {
            get
            {
                return connected;
            }
            set
            {
                connected = value;
            }
        }
        #endregion

        /*Конструктор класса*/
        public RemoteConnection()
        {
            this.device = Environment.MachineName;
            /*Инициализируем сокеты*/
            screenListener = new Socket(hostPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            screenSender = new Socket(hostPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            mediaListener = new Socket(hostPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            mediaSender = new Socket(hostPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            Task.Run(RunAsync);
        }

        /*Поток для приема подключений*/
        public async Task RunAsync()
        {
            try
            {
                var nat = new NatDiscoverer();
                var cts = new CancellationTokenSource(5000);
                // using SSDP protocol, it discovers NAT device.
                var device = await nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                // Пробрасываем порты для экрана
                await device.CreatePortMapAsync(new Mapping(Protocol.Udp, Global.receivePort, Global.receivePort, "ToServer port"));
                await device.CreatePortMapAsync(new Mapping(Protocol.Udp, Global.sendPort, Global.sendPort, "ToClient port"));
                //Пробрасываем порты для чата и звука
                await device.CreatePortMapAsync(new Mapping(Protocol.Udp, Global.communicationsSendPort, Global.communicationsSendPort, "Chat & Sound send port"));
                await device.CreatePortMapAsync(new Mapping(Protocol.Udp, Global.communicationsReceivePort, Global.communicationsReceivePort, "Chat & Sound receive port"));
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name != "NatDeviceNotFoundException")
                    DialogForm.Show("Ошибка", ex.Message, Global.DialogTypes.message);
            }
            try
            {        
                /*Привязываем сокеты к серверному адресу*/
                screenListener.Bind(hostPoint);
                mediaListener.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), Global.communicationsReceivePort));
                /*Ждем инициализации удаленного устройства*/
                await ConnectAsync();                
                /*Запускаем задачу чтения данных*/
                Task readCommands = new Task(Read);
                readCommands.Start();
                /*Запускаем задачу отправки данных*/
                Task writeCommands = new Task(Write);
                writeCommands.Start();
                screenActions.Start();
                /*Ожидаем завершения задач*/
                Task.WaitAll(readCommands, writeCommands);
            }
            catch (Exception ex)
            {
                DialogForm.Show("Ошибка", ex.ToString(), Global.DialogTypes.message);
            }
               
        }

        /*Выполняет все этапы подключения клиента к серверу*/
        private async Task ConnectAsync ()
        {
            while (true)
            {
                switch (access)
                {
                    #region INIT
                    /*Ждем сообщения INIT от клиента*/
                    case AccessLevel.None:
                        /*Считываем начальные данные об удаленном устройстве*/
                        EndPoint remoteIp = new IPEndPoint(IPAddress.Any, Global.receivePort);
                        /*Получаем сообщение*/
                        int bytes = 0; // количество полученных байтов
                        byte[] data = new byte[256]; // буфер для получаемых данных
                                                     /*Получаем данные и преобразуем их в DataSet*/
                        bytes = screenListener.ReceiveFrom(data, ref remoteIp);
                        DataSet initStructure = new DataSet(data,bytes);
                        /*Проверяем операцию
                          Как только отловили команду INIT, инициализируем удаленного пользователя*/
                        if (initStructure.command == DataSet.ConnectionCommands.INIT)
                        {
                            /*Получаем ip, с которого пришел сигнал*/
                            clientPoint = remoteIp as IPEndPoint;
                            clientPoint.Port = Global.sendPort;
                            remoteClient = new RemoteDevice((string)initStructure.variables[0], (string)initStructure.variables[1], clientPoint.Address);
                            /*Сразу после команды INIT на все рабочие порты (кроме основного) должна прийти HELLO-команда
                             для обеспечения связи по этим портам. Читаем ее*/
                            //IPEndPoint tmpPoint = new IPEndPoint(hostPoint.Address, Global.sendPort);
                            /*initStructure = ReadPackage(tmpPoint);
                            if(initStructure.command==DataSet.ConnectionCommands.HELLO)*/
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
                            passStructure = ReadPackage(hostPoint);
                            /*Проверяем операцию
                                Как только отловили команду PASSWORD, проверяем указанный в сообщении пароль*/
                            if (passStructure.command == DataSet.ConnectionCommands.PASSWORD)
                            {
                                if (passStructure.variables[0].Equals(Global.securityCode))
                                    access = AccessLevel.Connect;
                                else
                                {
                                    /*Пришел неверный пароль. Отправляем сообщение*/
                                    passStructure = new DataSet(DataSet.ConnectionCommands.EXIT);
                                    passStructure.Add("Неверный пароль");
                                    Send(passStructure);
                                    access = AccessLevel.None;
                                }
                            }
                            /*Пользователь передумал вводить пароль. Отклоняем это подключение и ждем нового*/
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
                    #region CONNECT
                    case AccessLevel.Connect:
                        /*Отправляем клиенту информацию об этом устройстве*/
                        DataSet connectStructure = new DataSet(DataSet.ConnectionCommands.CONNECT);
                        connectStructure.Add(this.username);
                        connectStructure.Add(this.device);
                        Send(connectStructure);
                        /*Получаем ответ от клиента*/
                        connectStructure = ReadPackage(hostPoint);
                        /*Если подключение успешно установлено, переходим к следующему шагу*/
                        if (connectStructure.command == DataSet.ConnectionCommands.CONNECT)
                            access = AccessLevel.SendData;
                        /*Иначе ждем нового подключения*/
                        else
                            access = AccessLevel.None;
                        break;
                    #endregion
                    /*Если получили доступ к передаче данных, выходим из метода инициализации*/
                    case AccessLevel.SendData:
                        Connected = true;
                        return;
                }
            }
        }

        /*Читает данные, получаемые от клиента в цикле и заносит их в очередь*/
        private void Read()
        {
            while (true)
            {
                while (screenListener.Available != 0)
                {
                    DataSet result = ReadPackage(hostPoint);
                    lock (screenActions.receiveQueue)
                        screenActions.receiveQueue.Enqueue(result);
                }
                Thread.Sleep(40);
            }
        }

        private void Write()
        {
            while(true)
            {
                /*Если в clientActions есть элементы, отправляем их клиенту*/
                while (screenActions.sendQueue.Count != 0)
                {
                    lock (screenActions.sendQueue)
                        Send(screenActions.sendQueue.Dequeue());
                    Thread.Sleep(2);
                }
                Thread.Sleep(40);
            }
        }

        /*Чтение одного пакета*/
        private DataSet ReadPackage (EndPoint point)
        {
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных
            /*Получаем данные и преобразуем их в DataSet*/
            bytes = screenListener.ReceiveFrom(data, ref point);
            return new DataSet(data, bytes);
        }

        /*Отправка данных*/
        public void Send(DataSet package)
        {
            byte[] data = package.ToByteArray();
            screenSender.SendTo(data, clientPoint);
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
