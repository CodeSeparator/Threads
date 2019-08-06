using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Python
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Start();
        }

        private void Start()
        {
            Python.InitScreen();
            Python.AddHare();
            int max = 20;
            Python[] p = new Python[max];
            Thread[] t = new Thread[max];
            for (int i = 0; i < max; i++)
            {
                p[i] = Python.Create();
                t[i] = new Thread(p[i].Run);
                t[i].IsBackground = true; // делаем поток фоновым
                //t[i].Priority = (ThreadPriority)i;// приоритеты
                t[i].Start();
                //t[i].Join();
            }
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if(key.KeyChar >= '0' && key.KeyChar <= '9')
                {
                    t[Convert.ToInt32(key.KeyChar.ToString())].Abort();
                }
            }
        }
    }
}
