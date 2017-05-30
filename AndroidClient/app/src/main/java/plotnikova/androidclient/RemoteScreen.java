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
import java.util.Queue;
import java.util.concurrent.ConcurrentLinkedQueue;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import static android.content.ContentValues.TAG;

/**
 * Created by Алёна on 11.05.2017.
 */

public class RemoteScreen extends SurfaceView implements SurfaceHolder.Callback {
    /*Поток, в котором будем рисовать*/
    private DrawThread drawThread;
    private ScheduledExecutorService drawer;
    /*юда передаем ссылку на очередь с частями для прорисовки*/
    private Queue<ScreenActions.ScreenPart> drawingQueue;
    /*Картинка, которую собираем из частей*/
    private Bitmap screenCapture;

    /*Ссылка на синглтон*/
    final Global global = Global.getInstance();

    /*Канвасы*/
    /*Канвас для рисования на вьюхе*/
    Canvas viewCanvas;
    /*Канвас для рисования на Bitmap'е*/
    Canvas captureCanvas;

    final Paint paint = new Paint(Paint.ANTI_ALIAS_FLAG);

    /*Готовность отображения снимка экрана*/
    private boolean imageReady = false;

    /*------КОНСТРУКТОРЫ------*/
    public RemoteScreen(Context ctx){
        super(ctx);
        global.screenActions.setView(this);
        getHolder().addCallback(this);
    }

    public RemoteScreen(Context context, AttributeSet attrs){
        super(context,attrs);
        global.screenActions.setView(this);
        getHolder().addCallback(this);
    }

    public RemoteScreen(Context context, AttributeSet attrs, int defStyle){
        super(context, attrs, defStyle);
        global.screenActions.setView(this);
        getHolder().addCallback(this);
    }

    /*------МЕТОДЫ------*/
    public void setDrawingQueue(Queue<ScreenActions.ScreenPart> queue){
        this.drawingQueue = queue;
    }

    public void setImageReady(boolean value){
        imageReady = value;
    }

    /*Для инициализации картинки по размерам*/
    public void setImageSize(int width, int height){
        screenCapture = Bitmap.createBitmap(width,height, Bitmap.Config.RGB_565);
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {
    }

    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        //captureCanvas = new Canvas();
        /*Ждем установки изображения*/
        while(global.screenActions.getScreenCapture()==null);
        /*Создаем планировщик на два потока*/
        drawThread = new DrawThread(holder);
        drawThread.setRunning(true);
        drawer = Executors.newScheduledThreadPool(1);
        drawer.scheduleWithFixedDelay(new DrawThread(holder),0,40, TimeUnit.MILLISECONDS);
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
            /*------------------------Рисуем на вьюхе------------------------*/
            /*Если еще нет доступа к канвасу, переходим к следующей итерации*/
            if(!surfaceHolder.getSurface().isValid())
                return;
            viewCanvas = surfaceHolder.lockCanvas();
            viewCanvas.drawBitmap(global.screenActions.getScreenCapture(),0,0,paint);
            if (viewCanvas != null) {
                    surfaceHolder.unlockCanvasAndPost(viewCanvas);
            }
            while(!surfaceHolder.getSurface().isValid());

        }
    }
}
