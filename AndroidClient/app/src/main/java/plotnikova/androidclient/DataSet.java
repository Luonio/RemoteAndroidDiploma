package plotnikova.androidclient;

import java.util.ArrayList;

/**
 * Created by Алёна on 27.04.2017.
 */

public class DataSet {

    public enum ConnectionCommands {
        NONE,
        INIT,
        PASSWORD,
        CONNECT,
        DECLINE,
        EXIT,
        ERROR
    }

    /*Получаемая/передаваемая команда*/
    public ConnectionCommands command;

    /*Массив данных*/
    public ArrayList<String> variables;

    /*Весь набор данных (КОМАНДА\данные,данные,данные...)*/
    public String pack;

    public DataSet() {
        command = ConnectionCommands.NONE;
        variables = new ArrayList<String>();
        pack = null;
    }

    public DataSet(String pack) {
        /*Разделяем строку на команду и данные*/
        fromString(pack);
    }

    public DataSet (Byte[] pack){
        fromString(pack.toString());
    }

    public DataSet (ConnectionCommands cmd){
        command = cmd;
        variables = new ArrayList<>();
        pack = toString(cmd);
    }

    /*Разбирает строку на компоненты*/
    private void fromString(String pack)
    {
        this.pack = pack;
        variables = new ArrayList<>();
        char slash = '\\';
        /*Разделяем строку на команду и данные*/
        String[] tmpArr = pack.split(String.valueOf(slash));
        /*Добавляем команду*/
        command = getCommand(tmpArr[0]);
        /*Получаем список переменных*/
        if(tmpArr[1]!=null)
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
            case "0x01":
                return ConnectionCommands.INIT;
            case "0x02":
                return ConnectionCommands.PASSWORD;
            case "0x03":
                return ConnectionCommands.CONNECT;
            case "0x04":
                return ConnectionCommands.DECLINE;
            case "0x05":
                return ConnectionCommands.EXIT;
            case "0x06":
                return ConnectionCommands.ERROR;
            default:
                return ConnectionCommands.NONE;
        }
    }

    /*Преобразует команду в строку*/
    private String toString(ConnectionCommands command)
    {
        switch(command)
        {
            case INIT:
                return "0x01\\";
            case PASSWORD:
                return "0x02\\";
            case CONNECT:
                return "0x03\\";
            case DECLINE:
                return "0x04\\";
            case EXIT:
                return "0x05\\";
            case ERROR:
                return "0x06\\";
            default:
                return "0x00\\";
        }
    }

    /*Преобразует строку данных в массив байтов*/
    public byte[] getBytes()
    {
        return this.pack.getBytes();
    }

    /*Добавление элемента данных к пакету*/
    public void add(Object value)
    {
        this.variables.add(value.toString());
        this.pack+=","+value.toString();
    }
}