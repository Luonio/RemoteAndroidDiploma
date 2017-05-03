package plotnikova.androidclient;

import android.app.Application;

/**
 * Created by Алёна on 02.05.2017.
 */

public final class Global {

    private String password;
    private DataSet.ConnectionCommands command;

    private static final Global instance = new Global();

    private Global() {
        password = new String();
        command = DataSet.ConnectionCommands.NONE;
    }

    public static Global getInstance() {
        return instance;
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
}
