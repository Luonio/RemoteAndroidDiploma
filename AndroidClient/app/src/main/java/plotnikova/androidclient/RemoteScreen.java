package plotnikova.androidclient;

import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.net.Uri;
import android.os.Environment;
import android.util.AttributeSet;
import android.util.Log;
import android.view.SurfaceView;
import android.view.SurfaceHolder;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.ArrayList;

import static android.content.ContentValues.TAG;

/**
 * Created by Алёна on 11.05.2017.
 */

public class RemoteScreen extends SurfaceView implements SurfaceHolder.Callback {
    /*Поток, в котором будем рисовать*/
    private DrawThread drawThread;
    /*Сюда передадим ссылку на массив частей экрана*/
    private ArrayList<ScreenActions.ScreenPart> drawingBuffer;

    /*Готовность отображения снимка экрана*/
    private boolean imageReady = false;

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

    public void setImageReady(boolean value){
        imageReady = value;
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {
    }

    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        drawThread = new DrawThread(holder);
        drawThread.setRunning(true);
        drawThread.start();
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {
        boolean retry = true;
        drawThread.setRunning(false);
        while (retry) {
            try {
                drawThread.join();
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
            /*Канвас для рисования на вьюхе*/
            Canvas viewCanvas;
            while(running){
                /*Если еще нет доступа к канвасу, переходим к следующей итерации*/
                if(!surfaceHolder.getSurface().isValid())
                    continue;
                    viewCanvas = surfaceHolder.lockCanvas();
                //if(imageReady)
                    synchronized (drawingBuffer) {
                        /*Перебираем пришедшие части экрана и рисуем новые*/
                        for (ScreenActions.ScreenPart part : drawingBuffer) {
                            if (part.isChanged()) {
                            /*Если в части уже прогрузилась картинка*/
                                if (part.image != null) {
                                    viewCanvas.drawBitmap(part.image, part.location.x,
                                            part.location.y, new Paint(Paint.ANTI_ALIAS_FLAG));
                                    part.setChanged(false);
                                }
                            }
                        }
                    }
                /*else
                    canvas.drawColor(Color.GRAY);*/
                if (viewCanvas != null) {
                        surfaceHolder.unlockCanvasAndPost(viewCanvas);
                }
            }
        }
    }
}
