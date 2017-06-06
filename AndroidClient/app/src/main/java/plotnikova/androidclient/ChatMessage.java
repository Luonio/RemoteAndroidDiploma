package plotnikova.androidclient;

import java.sql.Time;
import java.util.Date;

/**
 * Created by Алёна on 04.06.2017.
 */

/*Сообщение в чате*/
public class ChatMessage {

    //Тип сообщения
     public static enum MessageType{
         //Входящее. Отображается справа
         Incoming,
         //Исходящее. Отображается слева
         Outcoming,
         //Системное. Отображается по центру экрана
         System
     }

    //Имя пользователя, отправившего сообщение
    public String user;
    //Дата отправки
    public Date date;
    //Текст сообщения
    public String text;
    //Тип сообщения
    public MessageType type;

    /*По-умолчанию создается исходящее сообщение с текущим именем пользователя,
 временем, датой и заданным текстом*/
    public ChatMessage(String text)
    {
        user = Global.getInstance().getUsername();
        type = MessageType.Outcoming;
        date = new Date();
        this.text = text;
    }

    /*Сообщение, тип которого задается.
         Используется для инициализации объектов под входящие сообщения*/
    public ChatMessage(MessageType type)
    {
        this.type = type;
        date = new Date();
        text = "";
        if (type == MessageType.System)
            user = "System";
    }

    /*Сообщение, которому передаются тип и текст.
         Используется для вывода системных сообщений*/
    public ChatMessage (MessageType type, String text)
    {
        this.type = type;
        date = new Date();
        this.text = text;
        if (type == MessageType.System)
            user = "System";
    }
}
