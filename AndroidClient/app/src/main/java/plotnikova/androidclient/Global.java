package plotnikova.androidclient;

import android.app.Application;

/**
 * Created by Алёна on 02.05.2017.
 */

public final class Global {

    private String connectionPassword;

    private static final Global instance = new Global();

    private Global() {
        connectionPassword = new String();
    }

    public static Global getInstance() {
        return instance;
    }

    public String getPassword(){
        return connectionPassword;
    }

    public void setPassword(String psw){
        this.connectionPassword=psw;
    }
}
