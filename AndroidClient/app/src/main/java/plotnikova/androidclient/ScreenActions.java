package plotnikova.androidclient;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Point;
import android.view.SurfaceView;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Queue;
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

    /*Список частей экрана*/
    private ArrayList<ScreenPart> screen;
    /*Общее количество частей*/
    private int partsCount;
    /*Количество частей по-вертикали*/
    private int rows;
    /*Количество частей по-горизонтали*/
    private int cols;
    /*Размер части*/
    private int partWidth;
    private int partHeight;

    /*false, пока не получим команду SCREENINFO*/
    private boolean actionsAllowed;

    ScheduledExecutorService service;

    public ScreenActions(){
        sendQueue = new ConcurrentLinkedQueue<>();
        receiveQueue = new ConcurrentLinkedQueue<>();
        partsCount = 0;
        rows = 0;
        cols = 0;
        screen = new ArrayList<>();
        service = Executors.newScheduledThreadPool(2);
        actionsAllowed = false;
    }

    /*Установка SurfaceView, на которой будем рисовать*/
    public void setView(RemoteScreen v){
        this.view = v;
        view.setDrawingBuffer(this.screen);
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
        },0,1, TimeUnit.MILLISECONDS);
    }

    /*Выполняем команды из очереди, если она не пуста*/
    private void executeCommand() {
        synchronized (receiveQueue) {
            if (receiveQueue.size() != 0) {
                DataSet packet = receiveQueue.poll();
                switch (packet.command) {
                    case SCREENINFO:
                        partsCount = Integer.decode(packet.variables.get(0));
                        rows = Integer.decode(packet.variables.get(1));
                        cols = Integer.decode(packet.variables.get(2));
                        partWidth = Integer.decode(packet.variables.get(5));
                        partHeight = Integer.decode(packet.variables.get(6));
                        actionsAllowed = true;
                        /*Ждем, пока не загрузятся все части изображения*/
                        this.service.schedule(new Runnable() {
                            @Override
                            public void run(){
                                while(!imageIsReady())
                                    try {
                                        this.wait(500);
                                    }
                                    catch (InterruptedException ex){
                                        continue;
                                    }
                                view.setImageReady(true);
                                }
                        },5,TimeUnit.SECONDS);
                        break;
                    case SCREEN:
                        if (actionsAllowed) {
                            /*Получаем номер части*/
                            int partNumber = Integer.decode(packet.variables.get(0));
                            /*Если в списке еще нет элемента с таким номером, добавляем его
                            * и сортируем список*/
                            if(!contains(partNumber)) {
                                screen.add(new ScreenPart(packet, partWidth, partHeight));
                                Collections.sort(screen);
                            }
                            /*Иначе просто обновляем картинку*/
                            else
                                screen.get(partNumber).setImage(packet.variables.get(3));
                        }
                        break;
                }
            }
        }
    }

    /*Проверяет, есть ли в списке частей элемент с указанным номером*/
    private boolean contains(int num){
        for(ScreenPart part: screen){
            if(part.partNumber==num)
                return true;
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

        /*------КОНСТРУКТОРЫ------*/
        /*Получаем изображение из пакета*/
        public ScreenPart(DataSet packet, int width, int height){
            this.partNumber = new Integer(packet.variables.get(0));
            location = new Point(new Integer(packet.variables.get(1)),
                    new Integer(packet.variables.get(2)));
            this.width = width;
            this.height = height;
            /*Получаем массив байтов с изображением*/
            setImage(packet.variables.get(3));
        }

        /*------МЕТОДЫ------*/
        /*Устанавливает изображение части, декодированнное из строки вида
        * "число_число_число_...._число_число"*/
        public void setImage(String value) {
            byte[] byteImg = bytesFromString(value);
            this.image = BitmapFactory.decodeByteArray(byteImg, 0, byteImg.length);
            this.image = Bitmap.cre
            changed = true;
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
            StringBuilder builder = new StringBuilder();
            for(int i=0;i<str.length()-1;i+=2){
                builder.append(str.charAt(i));
                builder.append(str.charAt(i+1));
                bt.write(getByte(builder.toString()));
                builder.delete(0,1);
            }
            return bt.toByteArray();
        }

        /*Получаем байт из строки "ХХ"*/
        private byte getByte(String str)
        {
            int result = 0;
            for (int i=0;i<str.length();i++) {
                result*=0x10;
                switch (str.charAt(i)) {
                    case '1':
                        result+=001;
                        break;
                    case '2':
                        result+=0x02;
                        break;
                    case '3':
                        result+=0x03;
                        break;
                    case '4':
                        result+=0x04;
                        break;
                    case '5':
                        result+=005;
                        break;
                    case '6':
                        result+=0x06;
                        break;
                    case '7':
                        result+=0x07;
                        break;
                    case '8':
                        result+=0x08;
                        break;
                    case 'A':
                        result+=0x0A;
                        break;
                    case 'B':
                        result+=0x0B;
                        break;
                    case 'C':
                        result+=0x0C;
                        break;
                    case 'D':
                        result+=0x0D;
                        break;
                    case 'E':
                        result+=0x0E;
                        break;
                    case 'F':
                        result+=0x0F;
                        break;
                    default:
                        break;
                }
            }
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
