package plotnikova.androidclient;

import android.app.Application;

/**
 * Created by Алёна on 02.05.2017.
 */

public final class Global extends Application {
    private static final Global ourInstance = new Global();

    public static Global getInstance() {
        return ourInstance;
    }

    private Global() {
    }
}
