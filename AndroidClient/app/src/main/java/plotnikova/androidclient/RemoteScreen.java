package plotnikova.androidclient;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.util.AttributeSet;
import android.util.Log;
import android.view.SurfaceView;
import android.view.SurfaceHolder;

import java.util.ArrayList;

import static android.content.ContentValues.TAG;

/**
 * Created by Алёна on 11.05.2017.
 */

public class RemoteScreen extends SurfaceView implements SurfaceHolder.Callback {
    /*Поток, в котором будем рисовать*/
    private DrawThread thread;
    /*Сюда передадим ссылку на массив частей экрана*/
    private ArrayList<ScreenActions.ScreenPart> drawingBuffer;

    /*------КОНСТРУКТОРЫ------*/
    public RemoteScreen(Context ctx){
        super(ctx);
        Global.getInstance().screenActions.setView(this);
        getHolder().addCallback(this);
    }

    public RemoteScreen(Context context, AttributeSet attrs){
        super(context,attrs);
        Global.getInstance().screenActions.setView(this);
        getHolder().addCallback(this);
    }

    public RemoteScreen(Context context, AttributeSet attrs, int defStyle){
        super(context, attrs, defStyle);
        Global.getInstance().screenActions.setView(this);
        getHolder().addCallback(this);
    }

    /*------МЕТОДЫ------*/
    public void setDrawingBuffer(ArrayList<ScreenActions.ScreenPart> drawingBuffer) {
        this.drawingBuffer = drawingBuffer;
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

    /*Реализация потока*/
    class DrawThread extends Thread {
        private boolean running = false;
        private SurfaceHolder surfaceHolder;

        /*------КОНСТРУКТОРЫ------*/
        public DrawThread(SurfaceHolder surfaceHolder){
            this.surfaceHolder = surfaceHolder;
        }

        /*------МЕТОДЫ------*/
        public void setRunning(boolean running) {
            this.running = running;
        }

        @Override
        public void run(){
            /*Прежде всего ждем установки буфера*/
            while(drawingBuffer==null);
            Canvas canvas;
            while(running){
                /*Если еще нет доступа к канвасу, переходим к следующей итерации*/
                if(!surfaceHolder.getSurface().isValid())
                    continue;
                    canvas = surfaceHolder.lockCanvas();
                    /*Перебираем пришедшие части экрана и рисуем новые*/
                    for (ScreenActions.ScreenPart part: drawingBuffer) {
                        if(part.isChanged()) {
                            /*Если в части уже прогрузилась картинка*/
                            if(part.image!=null) {
                                canvas.drawBitmap(part.image, part.location.x,
                                        part.location.y, new Paint(Paint.ANTI_ALIAS_FLAG));
                                part.setChanged(false);
                            }
                    }
                }
                if (canvas != null) {
                        surfaceHolder.unlockCanvasAndPost(canvas);
                }
            }
        }
    }
}
