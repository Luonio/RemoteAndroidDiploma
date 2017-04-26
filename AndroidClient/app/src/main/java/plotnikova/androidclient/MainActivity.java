package plotnikova.androidclient;

import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.AppCompatActivity;
import android.view.Gravity;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.net.InetAddress;
import java.net.UnknownHostException;

public class MainActivity extends AppCompatActivity {

    private RemoteConnection connection;
    private ConnectionFragment connectionFragment;
    private ComputersFragment computersFragment;

    BottomNavigationView navigation;

    android.app.FragmentTransaction trans;

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
    }

    public void connectButton_onClick(View v)
    {
        String ip = connectionFragment.ipView.getText().toString();
        if(ip.equals("")) {
            Toast emptyIpError = Toast.makeText(getApplicationContext(),
                    "Введите IP-адрес!", Toast.LENGTH_SHORT);
            emptyIpError.setGravity(Gravity.BOTTOM, 0, navigation.getHeight());
            emptyIpError.show();
            return;
        }
        /*Если введенный ip корректен*/
        if(checkIP(ip)) {
            /*Получаем строки имени пользователя и кода безопасности*/
            String username = connectionFragment.nameView.getText().toString();
            String pass = connectionFragment.passwordView.getText().toString();
        }
        /*Если был введен некорректный ip-адрес*/
        else {
            Toast wrongIpError = Toast.makeText(getApplicationContext(),
                    "Некорректный IP-адрес!", Toast.LENGTH_SHORT);
            wrongIpError.setGravity(Gravity.BOTTOM, 0, navigation.getHeight());
            wrongIpError.show();
        }
    }

    /*Метод возвращает true, если введенная строка является ip-адресом
    * и false в противном случае*/
    Boolean checkIP(String adress)
    {
        try {
            InetAddress tmpAdress = InetAddress.getByName(adress);
        }
        catch (UnknownHostException ex) {
            return false;
        }
        return true;
    }

}
