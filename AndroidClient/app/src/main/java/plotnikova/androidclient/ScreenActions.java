package plotnikova.androidclient;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Point;
import android.util.Size;

import java.io.ByteArrayInputStream;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.Queue;
import java.util.concurrent.ConcurrentLinkedQueue;

/**
 * Created by Алёна on 08.05.2017.
 */

public class ScreenActions extends Thread {
    /*Очередь с командами на отправку*/
    public Queue<ScreenPart> sendQueue;
    /*Очередь с полученными командами*/
    public Queue<ScreenPart> receiveQueue;

    /*Список частей экрана*/
    private ArrayList<ScreenPart> screen;
    /*Общее количество частей*/
    private int partsCount;
    /*Количество частей по-вертикали*/
    private int rows;
    /*Количество частей по-горизонтали*/
    private int cols;
    /*Размер части*/
    private Size partSize;

    public ScreenActions(){
        sendQueue = new ConcurrentLinkedQueue<>();
        receiveQueue = new ConcurrentLinkedQueue<>();
    }

    class ScreenPart {
        /*Номер части*/
        public int partNumber;
        /*Координаты части*/
        public Point location;
        /*Размер части*/
        Size size;
        /*Изображение*/
        public Bitmap image;

        /*Получаем изображение из пакета*/
        public ScreenPart(DataSet packet){
            this.partNumber = new Integer(packet.variables.get(0));
            location = new Point(new Integer(packet.variables.get(1)),
                    new Integer(packet.variables.get(2)));
            /*Получаем массив байтов изображения*/
            byte[] bytes = ByteBuffer.allocate(packet.variables.get(3).length()).putLong(
                    new Long(packet.variables.get(3))).array();


        }
    }
}
