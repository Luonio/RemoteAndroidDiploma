package plotnikova.androidclient;

import android.app.Activity;
import android.content.Context;
import android.os.Build;

import java.net.Socket;

/**
 * Created by Алёна on 26.04.2017.
 */

/*Класс, хранящий данные об удаленном подключении*/
public class RemoteConnection {

    /*Родительская Activity*/
    private Context parent;

    /*Имя подключающегося пользователя*/
    public String username;

    /*Название подключающегося устройства*/
    public String device;

    /*Удаленный IP*/
    public String remoteIP;

    /*Код безопасности*/
    public String securityCode;

    /*Порт, по которому будет идти подключение*/
    public String remotePort;

    /*Имя удаленного устройства*/
    public String remoteDevice;

    /*Имя пользователя удаленного устройства*/
    public String remoteUsername;

    public Socket socket;

    public RemoteConnection(Context ctx)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = this.device;
        remoteIP="localhost";
        remotePort = parent.getString(R.string.remotePort);
    }

    public RemoteConnection(Context ctx, String username, String ip, String password)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = username;
        this.remoteIP = ip;
        this.securityCode = password;
        remotePort = parent.getString(R.string.remotePort);
    }

    public RemoteConnection (Context ctx, String ip, String password)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = device;
        this.remoteIP = ip;
        this.securityCode = password;
        remotePort = parent.getString(R.string.remotePort);
    }

    public RemoteConnection (Context ctx, String ip)
    {
        parent = ctx;
        this.device = Build.MANUFACTURER+Build.MODEL+" ("+Build.DEVICE+")";
        this.username = device;
        this.remoteIP = ip;
        remotePort = parent.getString(R.string.remotePort);
    }

}
