package plotnikova.androidclient;

import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.os.StrictMode;
import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.support.v7.app.AppCompatActivity;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.Toast;

import java.net.InetAddress;
import java.net.UnknownHostException;

public class MainActivity extends AppCompatActivity {

    private RemoteConnection connection;
    private ConnectionFragment connectionFragment;
    private ComputersFragment computersFragment;

    final Global global = Global.getInstance();

    BottomNavigationView navigation;

    android.app.FragmentTransaction trans;

    /*Константы для диалогов*/
    private static final int PASSWORD_DIALOG_ID = 0;

    private BottomNavigationView.OnNavigationItemSelectedListener mOnNavigationItemSelectedListener
            = new BottomNavigationView.OnNavigationItemSelectedListener() {

        @Override
        public boolean onNavigationItemSelected(@NonNull MenuItem item) {
            trans = getFragmentManager().beginTransaction();
            switch (item.getItemId()) {
                case R.id.navigation_connect:
                    trans.replace(R.id.content, connectionFragment).commit();
                    return true;
                case R.id.navigation_computers:
                    trans.replace(R.id.content, computersFragment).commit();
                    return true;
            }
            return false;
        }

    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        connectionFragment = new ConnectionFragment();
        computersFragment = new ComputersFragment();
        trans = getFragmentManager().beginTransaction();
        trans.add(R.id.content, connectionFragment);
        trans.commit();
        navigation = (BottomNavigationView) findViewById(R.id.navigation);
        navigation.setOnNavigationItemSelectedListener(mOnNavigationItemSelectedListener);
        global.mainHandler = new Handler(){
            public void handleMessage(Message msg){
                /*Константы для Handler'а*/
                final int WRONG_PASSWORD = 0;
                final int CONNECTION_DONE = 1;
                switch (msg.what)
                {
                    /*Нужно вывести сообщение о неправильном пароле*/
                    case WRONG_PASSWORD:
                        Toast wrongPassError = Toast.makeText(getApplicationContext(),
                                "Неверный пароль!", Toast.LENGTH_SHORT);
                        wrongPassError.setGravity(Gravity.BOTTOM, 0, navigation.getHeight());
                        wrongPassError.show();
                        break;
                    /*Ye;yj */
                    case CONNECTION_DONE:
                        Intent intent = new Intent(MainActivity.this,RemoteDesktopActivity.class);
                        MainActivity.this.startActivity(intent);
                        //TODO: запустить новую активити, передать ей объект connection, у connection сменить parent
                        break;
                }
                //TODO: заполнить обработку
            }
        };
        global.setToastHeight(navigation.getHeight());
    }

    public void connectButton_onClick(View v)
    {
        if(connection!=null) {
            /*TODO: реализовать повторное подключение без ошибки*/
            connection = null;
        }
        else {
            /*Разрешаем работу с сетью с основного потока*/
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitNetwork().build();
            StrictMode.setThreadPolicy(policy);
        }
        String ip = connectionFragment.ipView.getText().toString();
        if(ip.equals("")) {
            Toast emptyIpError = Toast.makeText(getApplicationContext(),
                    "Введите IP-адрес!", Toast.LENGTH_SHORT);
            emptyIpError.setGravity(Gravity.BOTTOM, 0, navigation.getHeight());
            emptyIpError.show();
            return;
        }
        /*Если введенный ip корректен*/
        try {
            global.screenActions = new ScreenActions();
            InetAddress adress = InetAddress.getByName(ip);
            /*Получаем строки имени пользователя и кода безопасности*/
            String username = connectionFragment.nameView.getText().toString();
            if(username.equals(""))
                global.remoteConnection = new RemoteConnection(this,adress);
            else
                global.remoteConnection = new RemoteConnection(this,username,adress);
            global.remoteConnection.startConnection();
        }
        /*Если был введен некорректный ip-адрес*/
        catch (UnknownHostException ex) {
            Toast wrongIpError = Toast.makeText(getApplicationContext(),
                    "Некорректный IP-адрес!", Toast.LENGTH_SHORT);
            wrongIpError.setGravity(Gravity.BOTTOM, 0, navigation.getHeight());
            wrongIpError.show();
        }
    }


    @Override
    protected Dialog onCreateDialog(int id) {
        android.app.AlertDialog dialog;
        switch(id)
        {
            case PASSWORD_DIALOG_ID:
                LayoutInflater inflater = (LayoutInflater) getSystemService(Context.LAYOUT_INFLATER_SERVICE);

                final View layout = inflater.inflate(R.layout.dialog_password, (ViewGroup) findViewById(R.id.passDialog));
                final EditText pass = (EditText) layout.findViewById(R.id.passwordText);

                android.app.AlertDialog.Builder builder = new android.app.AlertDialog.Builder(this);
                builder.setTitle("Enter Password");
                builder.setView(layout);

                /*Добавяляем кнопки*/
                //Отмена
                builder.setNegativeButton(android.R.string.cancel, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int whichButton) {
                        global.setCommand(DataSet.ConnectionCommands.EXIT);
                        removeDialog(PASSWORD_DIALOG_ID);
                    }
                });

                //ОК
                builder.setPositiveButton(android.R.string.ok, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int which) {
                        global.setCommand(DataSet.ConnectionCommands.PASSWORD);
                        global.setPassword(pass.getText().toString());
                        removeDialog(PASSWORD_DIALOG_ID);
                    }
                });
                dialog=builder.create();
                break;
            default:
                dialog=null;
        }
        return dialog;
    }


}

