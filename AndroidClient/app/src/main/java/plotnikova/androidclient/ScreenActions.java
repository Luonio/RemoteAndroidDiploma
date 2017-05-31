package plotnikova.androidclient;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.ImageFormat;
import android.graphics.Paint;
import android.graphics.Point;
import android.media.ImageReader;
import android.util.Base64;
import android.view.SurfaceView;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.lang.reflect.Array;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Queue;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.ConcurrentLinkedQueue;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

/**
 * Created by Алёна on 08.05.2017.
 */

public class ScreenActions {

    /*SurfaceView, на которой будем рисовать*/
    private RemoteScreen view;

    /*Очередь с командами на отправку*/
    public Queue<DataSet> sendQueue;
    /*Очередь с полученными командами*/
    public Queue<DataSet> receiveQueue;
    /*Очередь с частями для прорисовки*/
    private Queue<ScreenPart> drawingQueue;

    /*Картинка, которую собираем из частей*/
    private Bitmap screenCapture;
    /*Канвас для рисования на Bitmap'е*/
    Canvas captureCanvas;
    /*Для отрисовки изображения*/
    final Paint paint = new Paint(Paint.ANTI_ALIAS_FLAG);

    /*Список частей экрана*/
    private ArrayList<ScreenPart> screen;
    /*Общее количество частей*/
    private int partsCount;
    /*Количество загруженных частей*/
    protected int downloadedParts;
    /*Количество частей по-вертикали*/
    private int rows;
    /*Количество частей по-горизонтали*/
    private int cols;
    /*Размер части*/
    private int partWidth;
    private int partHeight;

    private boolean executing = true;

    /*false, пока не получим команду SCREENINFO или пока рисуем на вьюхе*/
    private boolean actionsAllowed;

    ScheduledExecutorService service;

    public ScreenActions(){
        sendQueue = new ConcurrentLinkedQueue<>();
        receiveQueue = new ConcurrentLinkedQueue<>();
        drawingQueue = new ConcurrentLinkedQueue<>();
        partsCount = 0;
        downloadedParts = 0;
        rows = 0;
        cols = 0;
        service = Executors.newScheduledThreadPool(1);
        actionsAllowed = false;
    }

    /*Установка SurfaceView, на которой будем рисовать*/
    public void setView(RemoteScreen v){
        this.view = v;
    }

    /*Начинаем работать с экраном*/
    public void startScreenActions(){
        /*Ждем инициализации вьюхи*/
        while(view==null);
        /*Выполняем команды, пришедшие с сервера*/
        service.scheduleWithFixedDelay(new Runnable() {
            @Override
            public void run() {
                executeCommand();
            }
        },0,40, TimeUnit.MILLISECONDS);
    }

    public void setExecuting(boolean value){
        this.executing = value;
    }


    public Bitmap getScreenCapture(){
        return this.screenCapture;
    }

    /*Выполняем команды из очереди, если она не пуста*/
    private void executeCommand() {
        DataSet packet;
        synchronized (receiveQueue) {
            while (receiveQueue.size() != 0) {
            /*if (receiveQueue.size() == 0)
                return;*/
                packet = receiveQueue.poll();
                try {
                    switch (packet.command) {
                        case SCREENINFO:
                            partsCount = Integer.decode(packet.variables.get(0));
                            rows = Integer.decode(packet.variables.get(1));
                            cols = Integer.decode(packet.variables.get(2));
                            partWidth = Integer.decode(packet.variables.get(5));
                            partHeight = Integer.decode(packet.variables.get(6));
                /*Инициализируем список частей*/
                            screen = new ArrayList<>(partsCount);
                            for (int i = 0; i < partsCount; i++) {
                                screen.add(new ScreenPart(i, partWidth, partHeight));
                            }
                /*Ждем, пока не определится вьюха для прорисовки,
                * затем инициализируем область для рисования*/
                            while (view == null) ;
                            actionsAllowed = true;
                            screenCapture = Bitmap.createBitmap(partWidth * cols, partHeight * rows, Bitmap.Config.RGB_565);
                            captureCanvas = new Canvas();
                            captureCanvas.setBitmap(screenCapture);
                            captureCanvas.drawColor(Color.BLACK);
                            break;
                        case SCREEN:
                            if (actionsAllowed) {
                    /*Получаем номер части*/
                                int partNumber = Integer.decode(packet.variables.get(0));
                    /*Устанавливаем изображение по указанному номеру*/
                                synchronized (screen) {
                                    screen.get(partNumber).setImage(packet.variables.get(3),
                                            new Point(new Integer(packet.variables.get(1)),
                                                    new Integer(packet.variables.get(2))));
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex){
                    ex.printStackTrace();
                }
            }
        }
    }



    /*Проверяет, есть ли в списке частей элемент с указанным номером*/
    private boolean contains(int num){
        synchronized (screen) {
            for (ScreenPart part : screen) {
                if (part.partNumber == num)
                    return true;
            }
        }
        return false;
    }

    private boolean imageIsReady(){
        boolean result = true;
        for(int i=0;i<partsCount;i++){
            if(!contains(i)) {
                DataSet askPart = new DataSet(DataSet.ConnectionCommands.SCREEN);
                askPart.add(i);
                sendQueue.offer(askPart);
                result = false;
            }
        }
        return result;
    }

    class ScreenPart implements Comparable<ScreenPart> {
        /*Номер части*/
        public int partNumber;
        /*Координаты части*/
        public Point location;
        /*Размер части*/
        int width;
        int height;
        /*Изображение*/
        public Bitmap image;

        private boolean changed;

        private Timer getImageTimer;

        /*------КОНСТРУКТОРЫ------*/
        /*Создаем объект с указанными размерами и номером*/
        public ScreenPart(int number, int width, int height){
            this.partNumber = number;
            this.width = width;
            this.height = height;
            getImageTimer = new Timer(true);
            /*роверяем изображение*/
            TimerTask getImageTask = new TimerTask() {
                @Override
                public void run() {
                    if(ScreenPart.this.image==null) {
                        DataSet askPart = new DataSet(DataSet.ConnectionCommands.SCREEN);
                        askPart.add(partNumber);
                        synchronized (sendQueue) {
                            sendQueue.offer(askPart);
                        }
                    }
                    else {
                        downloadedParts++;
                        getImageTimer.cancel();
                    }
                }
            };
            getImageTimer.schedule(getImageTask,0,2000);
        }

        /*------МЕТОДЫ------*/
        /*Устанавливает изображение части, декодированнное из строки вида
        * "число_число_число_...._число_число"*/
        public void setImage(String value, Point loc) {
            synchronized (this) {
                this.location = loc;
                byte[] byteImg = bytesFromString(value);
                this.image = BitmapFactory.decodeByteArray(byteImg, 0, byteImg.length);
                if(this.image!=null) {
                    ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
                    image.compress(Bitmap.CompressFormat.JPEG, 100, byteArrayOutputStream);
                    byte[] byteArray = byteArrayOutputStream .toByteArray();
                    String encoded = Base64.encodeToString(byteArray, Base64.DEFAULT);

                    captureCanvas.drawBitmap(this.image, this.location.x,
                            this.location.y, paint);
                }
            }
        }

        public void setChanged(boolean changed){
            this.changed = changed;
        }

        public boolean isChanged(){
            return changed;
        }

        /*Получаем массив байтов из строки*/
        private byte[] bytesFromString(String str){
            ByteArrayOutputStream bt = new ByteArrayOutputStream();
            String[] strArray = str.split("-");
            for(int i=0;i<strArray.length;i++){
                bt.write(getByte(strArray[i]));
            }
            return bt.toByteArray();
        }

        /*Получаем байт из строки "ХХ"*/
        private byte getByte(String str)
        {
            int result = Integer.parseInt(str,16);
            return (byte)result;

        }

        @Override
        public int compareTo(ScreenPart anotherPart){
            if(this.partNumber<anotherPart.partNumber)
                return -1;
            else if(this.partNumber>anotherPart.partNumber)
                return 1;
            return 0;
        }
    }
}
