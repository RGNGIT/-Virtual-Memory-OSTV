using System;
using System.Windows.Forms;
using System.Threading;

namespace __WM__OSTW {

    delegate void _ThreadTask(int TID);

    struct ThreadInfo {
        public bool Done;
        public int Time;
        public string ThreadName;
    }

    public partial class Form1 : Form {

        /// <Блок Первичная инициализация>

        static Mutex tMutex = new Mutex();
        static ThreadInfo[] tInfo = new ThreadInfo[5];

        public Form1() {

            InitializeComponent();

            for (int i = 0; i < 5; i++) {
                tInfo[i].Done = false;
                tInfo[i].ThreadName = $"Поток {i + 1}: ";
                Controls["textBox" + (i + 1)].Text = "Ожидание запуска...";
            }

        }

        /// <\><Блок Первичная инициализация>

        /// <Блок Таски однозадачных потоков>

        /// <Блок Инициализация или конкатенация делегата>

        _ThreadTask lTaskMngmt = tAction1; // Обязательно с int-овым параметром

        // lTaskMngmt += Имя Метода; ...

        void ThreadInit(object obj) {
            this.lTaskMngmt(Convert.ToInt32(obj));
        }

        /// <\><Блок Инициализация или конкатенация делегата>

        static void tAction1(int TID) {
            tMutex.WaitOne(); // Запрос и ожидание мьютекса
            Thread.Sleep(3000); tInfo[TID].Time--; // Тут типа чето делает
            if (tInfo[TID].Time <= 0) {
                tInfo[TID].Done = true;
            }
            tMutex.ReleaseMutex(); // Освободил мьютекс и вышел
        }

        /// <\><Блок Таски однозадачных потоков>

        /// <Блок События>

        int CalcPercent(int Times, int TID) {
            return (int)(100f - Math.Floor((float)tInfo[TID].Time / (float)Times * 100f));
        }

        void Percentage(int Times, int TID) {
            Controls["TextBox" + (TID + 6)].Text = $"{CalcPercent(Times, TID)}%";
            progressBar1.Value = CalcPercent(Times, TID);
        }

        bool tDone() {
            int Sum = 0;
            for (int i = 0; i < 5; i++) {
                Sum += Convert.ToInt32(tInfo[i].Done);
            }
            if (Sum == 5) {
                return true;
            } else {
                return false;
            }
        }

        private void Awake_Click(object sender, EventArgs e) {

            int[] Times = new int[5];
            for (int i = 0; i < 5; i++) {
                tInfo[i].Time = Convert.ToInt32(Controls["textBox" + (i + 11)].Text); Times[i] = tInfo[i].Time;
            }

            while (true) {
                for (int i = 0; i < 5; i++) {
                    if (!tInfo[i].Done) {
                        Thread tThread = new Thread(new ParameterizedThreadStart(ThreadInit));
                        tThread.Start(i);
                        Controls["textBox" + (i + 1)].Text = $"{tInfo[i].ThreadName}Выполняется. Запросил мьютекс.";
                        tThread.Join();
                        Controls["textBox" + (i + 1)].Text = $"{tInfo[i].ThreadName}Готов. Освободил мьютекс.";
                    }
                    if (tInfo[i].Done) {
                        Controls["textBox" + (i + 1)].Text = $"Готов. Выполнился за {Times[i] * 3000f}ms";
                    }
                    Percentage(Times[i], i);
                }
                if (tDone()) { break; }
            }
        }

        /// <\><Блок События>

    }
}
