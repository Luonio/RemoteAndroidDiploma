package plotnikova.androidclient;

import android.content.Context;
import android.util.AttributeSet;
import android.view.SurfaceView;
import android.view.SurfaceHolder;

import java.util.ArrayList;

/**
 * Created by Алёна on 11.05.2017.
 */

public class RemoteScreen extends SurfaceView implements SurfaceHolder.Callback {
    /*Поток, в котором будем рисовать*/
    private DrawThread thread;
    /*Сюда передадим ссылку на массив частей экрана*/
    //private ArrayList<ScreenActions.ScreenPart> drawingBuffer;

    /*------КОНСТРУКТОРЫ------*/
    public RemoteScreen(Context ctx){
        super(ctx);
        getHolder().addCallback(this);
    }

    public RemoteScreen(Context context, AttributeSet attrs){
        super(context,attrs);
        getHolder().addCallback(this);
    }

    public RemoteScreen(Context context, AttributeSet attrs, int defStyle){
        super(context, attrs, defStyle);
        getHolder().addCallback(this);
    }

    /*------МЕТОДЫ------*/
    public void setDrawingBuffer(ArrayList<ScreenActions.ScreenPart> drawingBuffer) {
        thread.setDrawingBuffer(drawingBuffer);
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {
    }

    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        thread = new DrawThread(holder);
        thread.setRunning(true);
        thread.start();
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {
        boolean retry = true;
        thread.setRunning(false);
        while (retry) {
            try {
                thread.join();
                retry = false;
            }
            catch (InterruptedException e) {}
        }
    }
}
