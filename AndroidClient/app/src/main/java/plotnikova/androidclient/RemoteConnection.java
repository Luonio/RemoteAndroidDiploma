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

/**
 * Created by Алёна on 26.04.2017.
 */

/*Класс, хранящий данные об удаленном подключении*/
public class RemoteConnection extends Thread {

    /*Родительская Activity*/
    private Context parent;
    /*Имя подключающегося пользователя*/
    public String username;
    /*Название подключающегося устройства*/
    public String device;
    /*Информация о сервере*/
    public RemoteHost host;
    /*Код безопасности*/
    public String securityCode;
    /*Сокет, по которому будем вести соединение*/
    DatagramSocket clientSocket;

    public RemoteConnection(Context ctx)
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

    public RemoteConnection (Context ctx, String username, InetAddress ip)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = device;
        /*Инициализируем сервер*/
        host = new RemoteHost(ip,Integer.parseInt(parent.getString(R.string.remotePort)));
        this.username = username;
    }

    public RemoteConnection (Context ctx, InetAddress ip)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = device;
        /*Инициализируем сервер*/
        host = new RemoteHost(ip, Integer.parseInt(parent.getString(R.string.remotePort)));
    }

    /*Отправка набора данных на удаленный адрес*/
    public void send(DataSet pack) {
        try {
            byte[] sendData = pack.getBytes();
            DatagramPacket sendPacket = new DatagramPacket(
                    sendData, sendData.length, host.ip, host.port);
            clientSocket.send(sendPacket);
        }
        catch(IOException ex) {
            /*TODO: добавить обработчик*/
        }
    }

    /*Вся работа потока*/
    @Override
    public void run()
    {
        try{
            clientSocket = new DatagramSocket();
        }
        catch (SocketException e) {
            /*TODO: заполнить обработку ошибки сокета*/
        }
        /*Устанавливаем соединение с сервером*/
        /*Шлем пакет инициализации*/
        DataSet initPack = new DataSet(DataSet.ConnectionCommands.INIT);
        initPack.add(username);
        initPack.add(device);
        send(initPack);
        clientSocket.close();
    }
}
