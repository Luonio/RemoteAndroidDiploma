package plotnikova.androidclient;

import android.animation.TypeConverter;
import android.support.annotation.NonNull;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.CharBuffer;
import java.nio.DoubleBuffer;
import java.nio.FloatBuffer;
import java.nio.IntBuffer;
import java.nio.LongBuffer;
import java.nio.ShortBuffer;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;

/**
 * Created by Алёна on 27.04.2017.
 */

public class DataSet {

    public enum ConnectionCommands {
        NONE,
        HELLO,
        INIT,
        PASSWORD,
        CONNECT,
        DECLINE,
        EXIT,
        ERROR,
        SCREEN,
        SCREENINFO
    }

    /*Получаемая/передаваемая команда*/
    public ConnectionCommands command;

    /*Массив данных*/
    public ArrayList<Object> variables;

    /*Весь набор данных (КОМАНДА\данные,данные,данные...)*/
    public byte[] pack;

    public DataSet() {
        command = ConnectionCommands.NONE;
        variables = new ArrayList<>();
        pack = null;
    }

    /*Получение пакета из массива байтов*/
    public DataSet (byte[] pack, int length){
        variables = new ArrayList<>();
        /*Получаем команду*/
        this.pack = pack;
        int position = 0;
        ByteArrayOutputStream buff = new ByteArrayOutputStream(5);
        buff.write(pack,position,5);
        String data = new String();
        position = 5;
        try {
            data = buff.toString("UTF-8");
        }
        catch (UnsupportedEncodingException ex) {
            //TODO: обработчик
        }
        this.command = getCommand(data);
        switch(command) {
            case EXIT:
                int len = length - 5;
                buff = new ByteArrayOutputStream(len);
                buff.write(pack,position,len);
                try {
                    data = buff.toString("UTF-8");
                }
                catch (UnsupportedEncodingException ex) {
                    //TODO: обработчик
                }
                this.add(data);
                break;
            case CONNECT:
                data = new String(pack,0,length);
                fromString(data);
                break;
            case SCREENINFO:
                //Количество частей
                short res = ByteBuffer.wrap(pack,position,2).order(ByteOrder.LITTLE_ENDIAN).getShort();
                this.variables.add(res);
                //2 байта плюс разделитель
                position+=3;
                //Количество строк
                res = ByteBuffer.wrap(pack,position,2).order(ByteOrder.LITTLE_ENDIAN).getShort();
                this.variables.add(res);
                position+=3;
                //Количество столбцов
                res = ByteBuffer.wrap(pack,position,2).order(ByteOrder.LITTLE_ENDIAN).getShort();
                this.variables.add(res);
                position+=3;
                //Ширина экрана
                int size = ByteBuffer.wrap(pack,position,4).order(ByteOrder.LITTLE_ENDIAN).getInt();
                this.variables.add(size);
                position+=5;
                //Высота экрана
                size = ByteBuffer.wrap(pack,position,4).order(ByteOrder.LITTLE_ENDIAN).getInt();
                this.variables.add(size);
                position+=5;
                //Ширина части
                size = ByteBuffer.wrap(pack,position,4).order(ByteOrder.LITTLE_ENDIAN).getInt();
                this.variables.add(size);
                position+=5;
                //Высота части
                size = ByteBuffer.wrap(pack,position,4).order(ByteOrder.LITTLE_ENDIAN).getInt();
                this.variables.add(size);
                break;
            case SCREEN:
                //Номер части
                short number = ByteBuffer.wrap(pack,position,2).order(ByteOrder.LITTLE_ENDIAN).getShort();
                this.variables.add(number);
                //2 байта плюс разделитель
                position+=3;
                //X
                //Ширина экрана
                int loc = ByteBuffer.wrap(pack,position,4).order(ByteOrder.LITTLE_ENDIAN).getInt();
                this.variables.add(loc);
                position+=5;
                //Y
                loc = ByteBuffer.wrap(pack,position,4).order(ByteOrder.LITTLE_ENDIAN).getInt();
                this.variables.add(loc);
                position+=5;
                //Картинка
                int pictArrayLength = length - position;
                ByteArrayOutputStream pictArray = new ByteArrayOutputStream(pictArrayLength);
                pictArray.write(pack,position,length-1);
                this.variables.add(pictArray.toByteArray());
                break;
            default:
                break;
        }
    }

    public DataSet (ConnectionCommands cmd){
        command = cmd;
        byte[] comm = new byte[5];
        variables = new ArrayList<>();
        try {
            comm = toString(cmd).getBytes("UTF-8");
        }
        catch (UnsupportedEncodingException ex) {
            //TODO: обработчик
        }
        pack = comm;
    }

    /*Разбирает строку на компоненты*/
    private void fromString(String pack)
    {
        /*Разделяем строку на команду и данные*/
        String[] tmpArr = pack.split("\\\\",2);
        /*Получаем список переменных*/
        if(tmpArr[1]!="")
            for(String val : tmpArr[1].split(","))
                variables.add(val);
    }

    /*Преобразование строки в команду*/
    private ConnectionCommands getCommand(String cmd)
    {
        switch(cmd)
        {
            /*0x01:remoteUsername,remoteDevice
                 var0 = username
                 var1 = device*/
            case "0x01\\":
                return ConnectionCommands.HELLO;
            case "0x02\\":
                return ConnectionCommands.INIT;
            case "0x03\\":
                return ConnectionCommands.PASSWORD;
            case "0x04\\":
                return ConnectionCommands.CONNECT;
            case "0x05\\":
                return ConnectionCommands.DECLINE;
            case "0x06\\":
                return ConnectionCommands.EXIT;
            case "0x07\\":
                return ConnectionCommands.ERROR;
            case "0x08\\":
                return ConnectionCommands.SCREEN;
            case "0x09\\":
                return ConnectionCommands.SCREENINFO;
            default:
                return ConnectionCommands.NONE;
        }
    }

    /*Преобразует команду в строку*/
    private String toString(ConnectionCommands command)
    {
        switch(command)
        {
            case HELLO:
                return "0x01\\";
            case INIT:
                return "0x02\\";
            case PASSWORD:
                return "0x03\\";
            case CONNECT:
                return "0x04\\";
            case DECLINE:
                return "0x05\\";
            case EXIT:
                return "0x06\\";
            case ERROR:
                return "0x07\\";
            case SCREEN:
                return "0x08\\";
            case SCREENINFO:
                return "0x09\\";
            default:
                return "0x00\\";
        }
    }

    /*Преобразует строку данных в массив байтов*/
    public byte[] getBytes()
    {
        return this.pack;
    }


    /*Добавление переменной типа short*/
    public void add(short value){
        ByteArrayOutputStream buffer = new ByteArrayOutputStream();
        buffer.write(pack,0,pack.length);
        try {
            if (!variables.isEmpty())
                buffer.write(",".getBytes(), 0, 1);
            buffer.write(ByteBuffer.allocate(2).order(ByteOrder.LITTLE_ENDIAN).putShort(value).array());
        }
        catch (IOException ex){
            //TODO: обработчик
        }
        pack = buffer.toByteArray();
        this.variables.add(value);
    }

    //Добавление массива байтов
    public void add(byte[] value){
        ByteArrayOutputStream buffer = new ByteArrayOutputStream();
        buffer.write(pack,0,pack.length);
        if (!variables.isEmpty())
            buffer.write(",".getBytes(), 0, 1);
        buffer.write(value,0,value.length);
        pack = buffer.toByteArray();
        this.variables.add(value);
    }

    //Добавление строки
    public void add(String value){
        ByteArrayOutputStream buffer = new ByteArrayOutputStream();
        buffer.write(pack,0,pack.length);
        if (!variables.isEmpty())
            buffer.write(",".getBytes(), 0, 1);
        try {
            byte[] tmp = value.getBytes("UTF-8");
            buffer.write(tmp, 0, tmp.length);
        }
        catch (UnsupportedEncodingException ex){
            //TODO: обработчик
        }
        pack = buffer.toByteArray();
        this.variables.add(value);
    }

    //Добавление переменной типа int
    public void add(int value){
        ByteArrayOutputStream buffer = new ByteArrayOutputStream();
        buffer.write(pack,0,pack.length);
        try {
            if (!variables.isEmpty())
                buffer.write(",".getBytes(), 0, 1);
            buffer.write(ByteBuffer.allocate(4).order(ByteOrder.LITTLE_ENDIAN).putInt(value).array());
        }
        catch (IOException ex){
            //TODO: обработчик
        }
        pack = buffer.toByteArray();
        this.variables.add(value);
    }
}
