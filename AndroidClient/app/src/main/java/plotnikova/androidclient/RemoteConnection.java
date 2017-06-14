package plotnikova.androidclient;

import android.app.Activity;
import android.content.Context;
import android.os.Build;


import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.Socket;
import java.net.SocketException;
import java.net.UnknownHostException;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;


/**
 * Created by Алёна on 26.04.2017.
 */

/*Класс, хранящий данные об удаленном подключении*/
public class RemoteConnection {

    /*Родительская Activity*/
    private Activity parent;
    /*Имя подключающегося пользователя*/
    public String username;
    /*Название подключающегося устройства*/
    public String device;
    /*Информация о сервере*/
    public RemoteHost host;
    /*Сокет, по которому будем вести соединение*/
    DatagramSocket sendScreenSocket;
    DatagramSocket receiveScreenSocket;
    DatagramSocket sendMediaSocket;
    DatagramSocket receiveMediaSocket;

    int receiveScreenPort = 0;
    int receiveMediaPort = 0;
    int sendScreenPort = 0;
    int sendMediaPort = 0;

    ScheduledExecutorService service;

    final Global global = Global.getInstance();

    final ScreenActions screenActions = global.screenActions;

    /*Константы для диалогов*/
    private static final int PASSWORD_DIALOG_ID = 0;

    /*---------КОНСТРУКТОРЫ---------*/
    public RemoteConnection(Activity ctx)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = this.device;
        global.setUsername(this.username);
        try {
            host = new RemoteHost(InetAddress.getLocalHost(), 0);
        }
        catch (UnknownHostException ex){
            /*TODO: заполнить обработку ошибки неизвестного хоста*/
        }
    }

    public RemoteConnection (Activity ctx, String username, InetAddress ip)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = username;
        global.setUsername(this.username);
        /*Инициализируем сервер*/
        host = new RemoteHost(ip,Integer.parseInt(parent.getString(R.string.sendPort)));
        receiveScreenPort = Integer.parseInt(parent.getString(R.string.receivePort));
        receiveMediaPort = Integer.parseInt((parent.getString(R.string.mediaReceivePort)));
        sendScreenPort = Integer.parseInt(parent.getString(R.string.sendPort));
        sendMediaPort = Integer.parseInt(parent.getString(R.string.mediaSendPort));
    }

    public RemoteConnection (Activity ctx, InetAddress ip)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = device;
        global.setUsername(this.username);
        /*Инициализируем сервер*/
        host = new RemoteHost(ip, Integer.parseInt(parent.getString(R.string.sendPort)));
        receiveScreenPort = Integer.parseInt(parent.getString(R.string.receivePort));
        receiveMediaPort = Integer.parseInt((parent.getString(R.string.mediaReceivePort)));
        sendScreenPort = Integer.parseInt(parent.getString(R.string.sendPort));
        sendMediaPort = Integer.parseInt(parent.getString(R.string.mediaSendPort));
    }

    /*---------МЕТОДЫ---------*/
    /*Устанавливает родительскую форму для класса*/
    public void setParent(Activity ctx){
        this.parent = ctx;
    }

    /*Устанавливаем соединение*/
    public void startConnection(){
        this.service = Executors.newScheduledThreadPool(3);
        service.submit(new Runnable(){
            @Override
            public void run(){
                try{
                    sendScreenSocket = new DatagramSocket();
                    receiveScreenSocket = new DatagramSocket(receiveScreenPort);
                    sendMediaSocket = new DatagramSocket();
                    receiveMediaSocket = new DatagramSocket(receiveMediaPort);
                }
                catch (SocketException e) {
                    /*TODO: заполнить обработку ошибки сокета*/
                }
                if(Connect()){
                    global.screenActions = new ScreenActions();
                    global.mainHandler.sendEmptyMessage(1);
                    global.screenActions.startScreenActions();
                    /*Читаем данные экрана*/
                    service.scheduleWithFixedDelay(new Runnable(){
                        @Override
                        public void run(){
                            readScreen();
                        }
                    },0,5, TimeUnit.MILLISECONDS);
                    /*Отправляем данные экрана, если очередь не пуста*/
                    service.scheduleWithFixedDelay(new Runnable(){
                        @Override
                        public void run(){
                            writeScreen();
                        }
                    },0,40,TimeUnit.MILLISECONDS);
                }
                else {
                    stopConnection();
                }
            }
        });
    }

    /*Закрываем сокеты и прекращаем работу executorService'a*/
    public void stopConnection(){
        service.shutdown();
        sendMediaSocket.close();
        receiveMediaSocket.close();
        sendScreenSocket.close();
        receiveScreenSocket.close();
    }

    /*Отправка набора данных через указанный сокет на указанный порт*/
    public void send(DataSet pack, DatagramSocket socket, int port) {

        try {
            byte[] sendData = pack.getBytes();
            DatagramPacket sendPacket = new DatagramPacket(
                    sendData, sendData.length, host.ip, port);
            socket.send(sendPacket);
        }
        catch(IOException ex) {
            /*TODO: добавить обработчик*/
        }
    }

    /*Получение набора данных с удаленного адреса*/
    public DataSet receive(DatagramSocket socket)    {
        try {
            byte[] receiveData = new byte[8192];
            DatagramPacket packet = new DatagramPacket(receiveData, receiveData.length);
            socket.receive(packet);
            return (new DataSet(packet.getData(),packet.getLength()));
        }
        catch (IOException e) {
            /*TODO: добавить обработчик*/
        }
        return null;
    }

    /*Постоянное чтение данных экрана*/
    private void readScreen(){
        DataSet result = receive(receiveScreenSocket);
        /*Блокируем очередь, чтобы не возникало ошибок при ее заполнении*/
        synchronized (global.screenActions.receiveQueue) {
            global.screenActions.receiveQueue.offer(result);
        }
    }

    /*Чтение сообщений чата и звука*/
    private void readMedia(){
        DataSet result = receive(receiveMediaSocket);
    }

    /*Запись данных экрана*/
    private void writeScreen(){
        synchronized (global.screenActions.sendQueue) {
            /*Пока очередь не пуста отправляем запросы*/
            while(global.screenActions.sendQueue.size()!=0)
                send(global.screenActions.sendQueue.poll(),sendScreenSocket, sendScreenPort);
        }
    }

    /*Отправка сообщений и звука*/
    private void writeMedia(){

    }

    private Boolean Connect() {
        int step=0;
        while(true) {
            switch (step) {
                //INIT
                case 0:
                    /*Шлем пакет инициализации*/
                    DataSet initPack = new DataSet(DataSet.ConnectionCommands.INIT);
                    initPack.add(username);
                    initPack.add(device);
                    send(initPack, sendScreenSocket, sendScreenPort);
                    //Инициализация портов. Для того, чтобы роутер (если есть) знал,
                    //с какими портами он работает. Иначе ответ на клиент не приходит
                    //TODO: добавить обратную пересылку сообщения
                    step++;
                    break;
                //PASSWORD/EXIT
                case 1:
                    DataSet passPack = receive(receiveScreenSocket);
                        /*Получили запрос пароля*/
                    if (passPack.command == DataSet.ConnectionCommands.PASSWORD) {
                            /*Открываем диалог в главном потоке*/
                        parent.runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                parent.showDialog(PASSWORD_DIALOG_ID);
                            }
                        });
                            /*Ждем, пока пользователь не введет пароль/выйдет из диалога*/
                        while (global.getCommand() == DataSet.ConnectionCommands.NONE)
                            ;
                            /*Проверяем команду*/
                        switch (global.getCommand()) {
                            case PASSWORD:
                                    /*Отправляем серверу пароль*/
                                passPack = new DataSet(DataSet.ConnectionCommands.PASSWORD);
                                passPack.add(global.getPassword());
                                send(passPack,sendScreenSocket,sendScreenPort);
                                step++;
                                break;
                            case EXIT:
                                    /*Отправляем серверу команду EXIT*/
                                passPack = new DataSet(DataSet.ConnectionCommands.EXIT);
                                send(passPack,sendScreenSocket,sendScreenPort);
                                return false;
                        }
                    }
                        /*Получили другую команду (CONNECT)*/
                    else {
                        step++;
                    }
                    break;
                    /*CONNECT/EXIT*/
                case 2:
                    DataSet connectPack = receive(receiveScreenSocket);
                    if(connectPack.command==DataSet.ConnectionCommands.CONNECT) {
                        host.username = (String)connectPack.variables.get(0);
                        host.device = (String)connectPack.variables.get(1);
                        connectPack = new DataSet(DataSet.ConnectionCommands.CONNECT);
                        send(connectPack,sendScreenSocket,sendScreenPort);
                        step++;
                        break;
                    }
                    else if(connectPack.command== DataSet.ConnectionCommands.EXIT) {
                        global.mainHandler.sendEmptyMessage(0);
                        return false;
                    }
                    /*Если инициализация основных портов прошла успешно,
                    * инициализируем медиа-порты*/
                case 3:
                     /*Шлем пакет инициализации*/
                    DataSet mediaPack = new DataSet(DataSet.ConnectionCommands.INIT);
                    send(mediaPack, sendMediaSocket, sendMediaPort);
                    /*Получаем ответ*/
                    mediaPack = receive(receiveMediaSocket);
                    if(mediaPack.command == DataSet.ConnectionCommands.INIT)
                        return true;
                    else
                        return false;
            }
        }
    }
}
