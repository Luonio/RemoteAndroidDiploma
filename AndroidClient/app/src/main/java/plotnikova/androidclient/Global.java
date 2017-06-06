package plotnikova.androidclient;

import android.app.Application;
import android.os.Handler;
import android.provider.SyncStateContract;

/**
 * Created by Алёна on 02.05.2017.
 */

public final class Global {

    private String username;
    private String password;
    private DataSet.ConnectionCommands command;
    private int toastHeight;
    public Handler mainHandler;

    public RemoteConnection remoteConnection;

    public ScreenActions screenActions;


    private static final Global instance = new Global();

    private Global() {
        password = new String();
        command = DataSet.ConnectionCommands.NONE;
        toastHeight = 0;
    }

    public static Global getInstance() {
        return instance;
    }

    public String getUsername(){
        return username;
    }
    public void setUsername(String value){
        this.username = value;
    }

    public String getPassword(){
        return password;
    }
    public void setPassword(String psw){
        this.password=psw;
    }

    public DataSet.ConnectionCommands getCommand(){
        return command;
    }
    public void setCommand (DataSet.ConnectionCommands cmd){
        this.command = cmd;
    }

    public int getToastHeight()
    {
        return toastHeight;
    }
    public void setToastHeight (int height)
    {
        toastHeight = height;
    }
}
