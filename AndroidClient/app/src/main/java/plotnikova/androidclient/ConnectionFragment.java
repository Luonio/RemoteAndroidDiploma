package plotnikova.androidclient;

import android.app.Activity;
import android.os.Bundle;
import android.app.Fragment;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;


public class ConnectionFragment extends Fragment {

    Activity parentActivity;

    Button connectButton;
    EditText nameView;
    EditText ipView;

    @Override
    public void onCreate (Bundle savedInstanceState){
        super.onCreate(savedInstanceState);
        parentActivity = getActivity();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        super.onCreateView(inflater, container, savedInstanceState);
        // Inflate the layout for this fragment
        View rootView = inflater.inflate(R.layout.fragment_connection, container, false);
        /*Получаем элменты*/
        connectButton = (Button) rootView.findViewById(R.id.connectButton);
        nameView = (EditText) rootView.findViewById(R.id.usernameText);
        ipView = (EditText) rootView.findViewById(R.id.ipText);
        return rootView;
    }
}
