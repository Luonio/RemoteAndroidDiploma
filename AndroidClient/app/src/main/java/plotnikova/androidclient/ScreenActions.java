package plotnikova.androidclient;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Point;
import android.util.Size;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
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

        /*КОНСТРУКТОРЫ*/
        /*Получаем изображение из пакета*/
        public ScreenPart(DataSet packet, Size sz){
            this.partNumber = new Integer(packet.variables.get(0));
            location = new Point(new Integer(packet.variables.get(1)),
                    new Integer(packet.variables.get(2)));
            this.size = sz;
            /*Получаем массив байтов с изображением*/
            setImage(packet.variables.get(3));
        }

        /*МЕТОДЫ*/
        /*Устанавливает изображение части, декодированнное из строки вида
        * "число_число_число_...._число_число"*/
        public void setImage(String value) {
            byte[] byteImg = bytesFromString(value);
            this.image = BitmapFactory.decodeByteArray(byteImg,0,byteImg.length);
        }

        /*Получаем массив байтов из строки*/
        private byte[] bytesFromString(String str){
            String[] stringArray = str.split("_");
            ByteArrayOutputStream bt = new ByteArrayOutputStream(partsCount);
            try {
                for (int i = 0; i < stringArray.length; i++) {
                    bt.write(stringArray[i].getBytes());
                }
            }
            catch (IOException ex){
                /*TODO: добавить обработчик*/
            }
            finally {
                return bt.toByteArray();
            }
        }
    }
}
