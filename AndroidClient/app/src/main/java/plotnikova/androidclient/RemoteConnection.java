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
    /*Код безопасности*/
    public String securityCode;
    /*Сокет, по которому будем вести соединение*/
    DatagramSocket sendSocket;
    DatagramSocket receiveSocket;

    ExecutorService service;

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
        this.username = device;
        /*Инициализируем сервер*/
        host = new RemoteHost(ip,Integer.parseInt(parent.getString(R.string.remotePort)));
        this.username = username;
    }

    public RemoteConnection (Activity ctx, InetAddress ip)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = device;
        /*Инициализируем сервер*/
        host = new RemoteHost(ip, Integer.parseInt(parent.getString(R.string.remotePort)));
    }

    /*---------МЕТОДЫ---------*/
    /*Устанавливает родительскую форму для класса*/
    public void setParent(Activity ctx){
        this.parent = ctx;
    }

    /*Устанавливаем соединение*/
    public void startConnection(){
        this.service = Executors.newCachedThreadPool();
        service.submit(new Runnable(){
            @Override
            public void run(){
                try{
                    sendSocket = new DatagramSocket();
                    receiveSocket = new DatagramSocket(host.port);
                }
                catch (SocketException e) {
                    /*TODO: заполнить обработку ошибки сокета*/
                }
                if(Connect()){
                    global.mainHandler.sendEmptyMessage(1);
                    global.screenActions.startScreenActions();
                    /*Читаем данные*/
                    service.submit(new Runnable(){
                        @Override
                        public void run(){
                            read();
                        }
                    });
                    /*Отправляем данные*/
                    service.submit(new Runnable(){
                        @Override
                        public void run(){
                            write();
                        }
                    });
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
        sendSocket.close();
        receiveSocket.close();
    }

    /*Отправка набора данных на удаленный адрес*/
    public void send(DataSet pack) {

        try {
            byte[] sendData = pack.getBytes();
            DatagramPacket sendPacket = new DatagramPacket(
                    sendData, sendData.length, host.ip, host.port);
            sendSocket.send(sendPacket);
        }
        catch(IOException ex) {
            /*TODO: добавить обработчик*/
        }
    }

    /*Получение набора данных с удаленного адреса*/
    public DataSet receive()    {
        try {
            byte[] receiveData = new byte[8192];
            DatagramPacket packet = new DatagramPacket(receiveData, receiveData.length, host.ip, host.port);
            receiveSocket.receive(packet);
            String msg = new String(packet.getData(), packet.getOffset(), packet.getLength());
            return (new DataSet(msg));
        }
        catch (IOException e) {
            /*TODO: добавить обработчик*/
        }
        return null;
    }

    /*Постоянное чтение данных*/
    private void read(){
        while(true) {
            DataSet result = receive();
            /*Блокируем очередь, чтобы не возникало ошибок при ее заполнении*/
            synchronized (global.screenActions.receiveQueue) {
                global.screenActions.receiveQueue.offer(result);
            }
        }
    }

    /*Постоянная запись данных*/
    private void write(){
        while(true) {
            /*Если очередь не пуста*/
            if(global.screenActions.sendQueue.size()!=0)
                synchronized (global.screenActions.sendQueue) {
                    send(global.screenActions.sendQueue.poll());
                }
        }
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
                    send(initPack);
                    step++;
                    break;
                //PASSWORD/EXIT
                case 1:
                    DataSet passPack = receive();
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
                                send(passPack);
                                step++;
                                break;
                            case EXIT:
                                    /*Отправляем серверу команду EXIT*/
                                passPack = new DataSet(DataSet.ConnectionCommands.EXIT);
                                send(passPack);
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
                    DataSet connectPack = receive();
                    if(connectPack.command==DataSet.ConnectionCommands.CONNECT) {
                        host.username = connectPack.variables.get(0);
                        host.device = connectPack.variables.get(1);
                        connectPack = new DataSet(DataSet.ConnectionCommands.CONNECT);
                        send(connectPack);
                        step++;
                        break;
                    }
                    else if(connectPack.command== DataSet.ConnectionCommands.EXIT) {
                        global.mainHandler.sendEmptyMessage(0);
                        return false;
                    }
                    /*Если инициализация прошла успешно, возвращаем true*/
                case 3:
                    return true;
            }
        }
    }
}
