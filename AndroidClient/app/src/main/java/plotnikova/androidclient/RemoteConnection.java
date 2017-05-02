package plotnikova.androidclient;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Build;
import android.os.Handler;
import android.os.Message;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.LinearLayout;

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
    DatagramSocket clientSocket;
    DatagramSocket receiveSocket;

    /*Константы для диалогов*/
    private static final int PASSWORD_DIALOG_ID = 0;

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

    /*Получение набора данных с удаленного адреса*/
    public DataSet receive()
    {
        try {
            byte[] receiveData = new byte[1024];
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

    /*Вся работа потока*/
    @Override
    public void run()
    {
        try{
            clientSocket = new DatagramSocket();
            receiveSocket = new DatagramSocket(host.port);
        }
        catch (SocketException e) {
            /*TODO: заполнить обработку ошибки сокета*/
        }
        /*Если получается успешно подключиться, начинаем удаленный сеанс*/
        if(Connect())
        {
            while(true);
        }
        clientSocket.close();
    }


    private Boolean Connect()
    {

        /*INIT*/
        /*Шлем пакет инициализации*/
        DataSet initPack = new DataSet(DataSet.ConnectionCommands.INIT);
        initPack.add(username);
        initPack.add(device);
        send(initPack);
        /*end INIT*/

        /*PASSWORD & CONNECT*/
        DataSet passPack = receive();
        /*Получили запрос пароля*/
        if(passPack.command == DataSet.ConnectionCommands.PASSWORD) {
            parent.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    parent.showDialog(PASSWORD_DIALOG_ID);
                }
            });
        }
        /*end PASSWORD & CONNECT*/
        return true;
    }

    /*@Override
    protected Dialog onCreateDialog(int id) {
        AlertDialog dialog;
        switch(id)
        {
            case PASSWORD_DIALOG_ID:
                LayoutInflater inflater = (LayoutInflater) parent.getSystemService(Context.LAYOUT_INFLATER_SERVICE);

                final View layout = inflater.inflate(R.layout.dialog_password, (ViewGroup) parent.findViewById(R.id.passDialog));
                final EditText pass = (EditText) layout.findViewById(R.id.passwordText);

                AlertDialog.Builder builder = new AlertDialog.Builder(parent);
                builder.setTitle("Enter Password");
                builder.setView(layout);

                /*Добавяляем кнопки*/
                //Отмена
                /*builder.setNegativeButton(android.R.string.cancel, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int whichButton) {
                        /*TODO: реализовать отправку команты CANCEL серверу*/
                        /*parent.removeDialog(PASSWORD_DIALOG_ID);
                    }
                });

                //ОК
                builder.setPositiveButton(android.R.string.ok, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int which) {
                        String password = pass.getText().toString();
                        /*TODO: реализовать отправку пароля серверу*/
                       /* parent.removeDialog(PASSWORD_DIALOG_ID);
                    }
                });
                dialog=builder.create();
                break;
            default:
                dialog=null;
        }
        return dialog;
    }*/
}
