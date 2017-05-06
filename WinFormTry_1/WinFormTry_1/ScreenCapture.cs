using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.ComponentModel;

namespace WinFormTry_1
{
    /*Класс для работы со снимком экрана*/
    class ScreenCapture
    {
        #region Поля
        /*Снимок экрана в момент времени*/
        public volatile Bitmap capture;

        /*Поток, выполняющий работу с изображением*/
        private Thread captureThread;

        #endregion

        #region Конструкторы
        public ScreenCapture()
        {
            capture = Capture();
            captureThread = new Thread(CaptureScreen);
            captureThread.IsBackground = true;
        }
        #endregion

        #region Методы

        /*Запуск демонстрации экрана*/
        public void Start()
        {
            /*Если выполнение потока приостановлено, запускаем его*/
            if (captureThread.ThreadState.HasFlag(ThreadState.Suspended))
                captureThread.Resume();
            /*Иначе запускаем новый поток*/
            else
                captureThread.Start();
        }

        /*Прекращение демонстрации экрана*/
        public void Stop()
        {
            captureThread.Suspend();
        }

        /*Делает снимок экрана, сравнивает его с предыдущим снимком и фиксирует изменения*/
        private async void CaptureScreen()
        {
            while(true)
            {
                /*Снимок экрана каждые 40мс*/
                Thread.Sleep(30000);
                await Task.Run(SaveChanges);
            }
        }

        /*Получаем новый снимок экрана и сравниваем его с предыдущим*/
        private async Task SaveChanges()
        {
            Bitmap currentScreen = Capture();
            DataSet package = new DataSet(DataSet.ConnectionCommands.SCREEN);
            /*TODO: реализовать сравнение двух изображений (текущего снимка экрана и предыдущего)
                    и записать результат*/

        }

        public static DataSet GetScreenshot()
        {
            /*Создаем новый пакет*/
            DataSet package = new DataSet(DataSet.ConnectionCommands.FULLSCREEN);
            /*Делаем снимок экрана*/
            Bitmap currentScreen = Capture();
            /*Добавляем в пакет положение курсора на экране*/
            package.Add(Cursor.Position.X);
            package.Add(Cursor.Position.Y);
            /*Преобразуем снимок экрана в массив байтов*/
            MemoryStream ms = new MemoryStream();
            currentScreen.Save(ms, ImageFormat.Jpeg);
            byte[] buffer = ms.ToArray();
            /*Добавляем в пакет число, представляющее наше изображение*/
            package.Add(BitConverter.ToUInt64(buffer, 0));
            UInt64 a = BitConverter.ToUInt64(buffer, 0);
            byte[] tryBuffer = BitConverter.GetBytes(a);

            
            ms.Dispose();
            return package;
        }

        /*Снимок экрана*/
        public static Bitmap Capture()
        {
            /*Получаем снимок экрана*/
            Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            /*Рисуем курсор*/
            Graphics g = Graphics.FromImage(screenshot);
            g.CopyFromScreen(0, 0, 0, 0, screenshot.Size);
            if (Cursor.Current != null)
                using (Icon cursor = Icon.FromHandle(Cursor.Current.Handle))
                    g.DrawIcon(cursor, new Rectangle(Cursor.Position, cursor.Size));
            g.Dispose();
            return screenshot;
        }
        #endregion
    }
}
