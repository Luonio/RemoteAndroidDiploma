package plotnikova.androidclient;

import java.net.InetAddress;
import java.net.Socket;

/**
 * Created by Алёна on 27.04.2017.
 */

public class RemoteHost {
    /*Удаленный IP*/
    public InetAddress ip;
    /*Имя удаленного устройства*/
    public String device;
    /*Имя пользователя удаленного устройства*/
    public String username;
    /*Порт*/
    public int port;

    public RemoteHost(InetAddress ip, int port)
    {
        this.ip = ip;
        this.port = port;
    }

}
