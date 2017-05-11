package plotnikova.androidclient;

import android.graphics.Canvas;
import android.graphics.Paint;
import android.view.SurfaceHolder;

import java.util.ArrayList;

/**
 * Created by Алёна on 11.05.2017.
 */

/*Реализация потока*/
class DrawThread extends Thread {
    private boolean running = false;
    private SurfaceHolder surfaceHolder;
    private ArrayList<ScreenActions.ScreenPart> drawingBuffer;

    /*------КОНСТРУКТОРЫ------*/
    public DrawThread(SurfaceHolder surfaceHolder){
        this.surfaceHolder = surfaceHolder;
    }

    /*------МЕТОДЫ------*/
    public void setRunning(boolean running) {
        this.running = running;
    }

    public void setDrawingBuffer(ArrayList<ScreenActions.ScreenPart> array)
    {
        this.drawingBuffer = array;
    }

    @Override
    public void run(){
            /*Прежде всего ждем установки буфера*/
        while(drawingBuffer==null);
        Canvas canvas;
        while(running){
            canvas=null;
            try{
                canvas = surfaceHolder.lockCanvas(null);
                if(canvas==null)
                    continue;
                    /*Перебираем пришедшие части экрана и рисуем новые*/
                for (ScreenActions.ScreenPart part: drawingBuffer) {
                    if(part.isChanged()) {
                        canvas.drawBitmap(part.image, part.location.x,
                                part.location.y, new Paint(Paint.ANTI_ALIAS_FLAG));
                        part.setChanged(false);
                    }
                }
            }
            finally{
                if (canvas != null) {
                    surfaceHolder.unlockCanvasAndPost(canvas);
                }
            }
        }
    }
}
