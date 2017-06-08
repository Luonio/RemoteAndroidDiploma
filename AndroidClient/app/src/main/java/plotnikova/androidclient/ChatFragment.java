package plotnikova.androidclient;

import android.content.Context;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.TextView;

import java.util.ArrayList;

public class ChatFragment extends Fragment {

    public ListView mList;
    public MessageAdapter mAdapter;
    public ArrayList<ChatMessage> messages;

    public ChatFragment() {
        messages = new ArrayList<>();
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        final View view =  inflater.inflate(R.layout.fragment_chat, container, false);
        mAdapter = new MessageAdapter(view.getContext(), messages);
        mList = (ListView)view.findViewById(R.id.messageArea);
        mList.setAdapter(mAdapter);
        registerForContextMenu(mList);
        Button button = (Button) view.findViewById(R.id.sendButton);
        button.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                String messageText = ((EditText)view.findViewById(R.id.enterBox)).getText().toString();
                if(messageText!=""){
                    ChatMessage message = new ChatMessage(messageText);
                    messages.add(message);
                    mAdapter.notifyDataSetChanged();
                }

            }
        });
        return view;
    }
}
