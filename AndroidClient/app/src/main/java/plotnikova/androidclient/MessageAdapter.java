package plotnikova.androidclient;

import android.annotation.TargetApi;
import android.content.Context;
import android.os.Build;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import java.util.ArrayList;

/**
 * Created by Алёна on 07.06.2017.
 */

/*Адаптер для подгрузки сообщений*/
public class MessageAdapter extends BaseAdapter {
    Context ctx;
    LayoutInflater inflater;
    ArrayList<ChatMessage> objects;

    public MessageAdapter(Context context, ArrayList<ChatMessage> messages){
        ctx = context;
        objects = messages;
        inflater = (LayoutInflater) ctx.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    }

    // кол-во элементов
    @Override
    public int getCount() {
        return objects.size();
    }

    // элемент по позиции
    @Override
    public Object getItem(int position) {
        return objects.get(position);
    }

    // id по позиции
    @Override
    public long getItemId(int position) {
        return position;
    }

    // пункт списка
    @Override
    @TargetApi(Build.VERSION_CODES.M)
    public View getView(int position, View convertView, ViewGroup parent) {
        View view = convertView;
        if(view==null){
            view = inflater.inflate(R.layout.layout_chat_message,parent,false);
        }
        ChatMessage mess = getMessage(position);
        /*Устанавливаем фоновый цвет вьюхи в зависимости от типа сообщения*/
        if(mess.type == ChatMessage.MessageType.Incoming)
            view.setBackgroundColor(ctx.getColor(R.color.incomingMessageColor));
        else if(mess.type == ChatMessage.MessageType.Outcoming)
            view.setBackgroundColor(ctx.getColor(R.color.outcomingMessageColor));
        /*Заполняем вьюху данными*/
        ((TextView)view.findViewById(R.id.timeField)).setText(mess.date.toString());
        ((TextView)view.findViewById(R.id.messageField)).setText(mess.text);

        return view;
    }

    ChatMessage getMessage(int position){
        return ((ChatMessage) getItem(position));
    }
}
