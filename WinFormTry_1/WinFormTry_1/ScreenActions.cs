﻿using System;
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
using System.Collections;

namespace WinFormTry_1
{
    /*Класс для работы со снимком экрана*/
    public class ScreenActions
    {
        #region Поля

        /*Действия, полученные от клиента для реализации на сервере*/
        public Queue<DataSet> sendQueue;
        /*Дествия, отправляемые сервером для реализации на клиенте*/
        public Queue<DataSet> receiveQueue;

        /*Снимок экрана в момент времени*/
        public volatile Bitmap capture;

        #region Части экрана
        /*Список частей экрана*/
        private List<ScreenPart> screen;
        /*Общее количество частей*/
        private int partsCount;
        /*Количество частей по-вертикали*/
        private int rows;
        /*Количество частей по-горизонтали*/
        private int cols;
        /*Размер части*/
        private Size partSize;
        #endregion

        /*Поток, выполняющий работу с экраном*/
        private Thread captureThread;

        #endregion

        #region Конструкторы
        public ScreenActions()
        {
            Divide();
            captureThread = new Thread(CaptureScreen);
            captureThread.IsBackground = true;
            this.sendQueue = new Queue<DataSet>();
            this.receiveQueue = new Queue<DataSet>();
        }
        #endregion

        #region Методы

        #region Снимок экрана
        /*Получаем n частей экрана, где n - наименьшее не простое число, 
         получаемое при округлении от деления размера буфера изображения на 8кб*/
        private void Divide()
        {
            /*Получаем изображение*/
            capture = Capture();
            /*Преобразуем снимок экрана в массив байтов*/
            MemoryStream ms = new MemoryStream();
            capture.Save(ms, ImageFormat.Jpeg);
            int len = Convert.ToInt32(ms.Length);
            /*Получаем partsCount частей по 8кб*/
            partsCount = len / 8192;
            /*Увеличиваем число частей в 10 раз, так как размер каждой части может меняться*/
            partsCount *= 10;
            /*Пока не получим не простое число, увеличиваем его на единицу*/
            while (IsSimple(partsCount))
                partsCount++;
            /*Получаем делители числа*/
            GetMultipliers(partsCount, ref cols, ref rows);
            /*Получаем размер частей*/
            partSize = new Size(capture.Width / cols, capture.Height / rows);
            /*Инициализируем список частей снимка экрана*/
            screen = new List<ScreenPart>();
            for (int i = 0; i < partsCount; i++)
            {
                screen.Add(new ScreenPart(i, new Point((i % cols) * partSize.Width,
                    (i / cols) * partSize.Height), partSize, capture));
            }

        }
        private static Bitmap Capture()
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

        /*Обновляем список частей*/
        private void Refresh()
        {
            foreach (ScreenPart part in screen)
                part.SetImage(capture);
        }

        /*Отмечает все части как измененные для отправки изображения целиком*/
        private void CheckAll()
        {
            foreach (ScreenPart part in screen)
                part.changed = true;
        }

        public void MakeScreenshot()
        {
            this.capture = Capture();
            Refresh();
        }
        #endregion

        #region Демонстрация экрана
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

        /*Поток для работы с экраном.
          - Отправляет клиенту данные об экране компьютера и ждет ответа
          - Отправляет зафиксированные изменения
          - Получает и выполняет команды, связанные с экраном, от клиента*/
        private async void CaptureScreen()
        {
            /*Данные об экране:
              - количество частей
              - размер снимка экрана
              - размер части снимка экрана*/
            DataSet screenInfo = new DataSet(DataSet.ConnectionCommands.SCREENINFO);
            screenInfo.Add(partsCount);
            screenInfo.Add(rows);
            screenInfo.Add(cols);
            screenInfo.Add(capture.Width);
            screenInfo.Add(capture.Height);
            screenInfo.Add(partSize.Width);
            screenInfo.Add(partSize.Height);
            lock (sendQueue)
                sendQueue.Enqueue(screenInfo);
            Thread.Sleep(100);
            MakeScreenshot();
            CheckAll();
            while (true)
            {
                /*Делаем снимок экрана, чтобы отправлялась обновленная картинка*/
                MakeScreenshot();
                Thread.Sleep(Global.connectionInterval);                
            }
        }
        #endregion

        /*Выполнение полученных от клиента команд*/
        public async Task ExecuteActionsAsync()
        {
            if (receiveQueue.Count != 0)
            {
                DataSet currentAction;
                lock (receiveQueue)
                    currentAction = receiveQueue.Dequeue();
                switch (currentAction.command)
                {
                    //TODO: сделать переключатель действий для первой команды в очереди
                }
            }
        }

        #region Математика
        /*Возвращает true, если число простое
          w, h - числа, при умножении которых можно получить n
          если n - простое число, w=h=0*/
        private static bool IsSimple(int n)
        {
            for (int i = 2; i < n / 2; i++)
                if (n % i == 0)
                    return false;
            return true;
        }

        /*Находит наиболее близкие по значению делители числа n*/
        private static void GetMultipliers(int n, ref int a, ref int b)
        {
            a = Convert.ToInt32(Math.Sqrt(n));
            b = a;
            while (!IsDivider(n, b))
                b--;
            a = n / b;
        }

        /*Возвращает true, если а является делителем числа n*/
        private static bool IsDivider(int n, int a)
        {
            if (n % a == 0)
                return true;
            return false;
        }
        #endregion

        #endregion
    }

    /*Отдельно обрабатываемая часть экрана*/
    class ScreenPart
    {
        #region Поля
        /*Номер части*/
        public int partNumber;
        /*Координаты части*/
        public Point location;
        /*Размер части*/
        Size size;
        /*Изображение*/
        public Bitmap image;

        /*Отслеживает изменения на экране*/
        public bool changed;
        /*Таймер, фиксирующий изменения на экране*/
        System.Timers.Timer fixChangesTimer;
        #endregion

        #region Конструкторы
        public ScreenPart(int num, Point loc, Size size, Bitmap screen)
        {
            this.changed = false;
            this.partNumber = num;
            this.location = loc;
            this.size = size;
            SetImage(screen);
            /*Заводим таймер отслеживания изменений*/
            fixChangesTimer = new System.Timers.Timer(Global.connectionInterval);
            fixChangesTimer.Elapsed += FixChangesTimer_Elapsed;
            fixChangesTimer.Start();
        }

        #endregion

        #region Методы
        /*Устанавливает изображение, вырезанное с экрана.
          screen - целый скриншот экрана*/
        public void SetImage(Bitmap screen)
        {
            image = new Bitmap(screen.Clone(new Rectangle(location, size), screen.PixelFormat));
        }


        /*Упаковывает изображение части экрана в DataSet*/
        public DataSet ToDataSet()
        {
            DataSet data = new DataSet(DataSet.ConnectionCommands.SCREEN);
            data.Add(this.partNumber);
            data.Add(this.location.X);
            data.Add(this.location.Y);
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);            
            byte[] buffer = new byte[ms.Length];
            buffer = ms.GetBuffer();
            data.Add(buffer.GetString());
            ms.Dispose();
            return data;
        }

        /*Событие по тикам таймера отслеживания изменений*/
        private void FixChangesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (changed)
            {
                lock (Global.screenActions.sendQueue)
                    Global.screenActions.sendQueue.Enqueue(this.ToDataSet());
                changed = false;
            }
        }
        #endregion
    }
}