using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Python
{
    public struct Coord
    {
        public int x;
        public int y;
        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Coord Plus(Coord c)
        {
            return new Coord(this.x + c.x, this.y + c.y);
        }
        public static Coord operator +(Coord c1, Coord c2)
        {
            return new Coord(c1.x + c2.x, c1.y + c2.y);
        }
    }
    class Python
    {
        public static readonly Coord size = new Coord(70, 24);
        public static readonly char aNone = ' ';
        public static readonly char aWall = '#';
        public static readonly char aBody = 'O';

        internal void Show()
        {

            PutScreen(head, color, aHead[0]);
        }

        private void ShowMe(Coord cHead, Coord cBody, Coord cNone)
        {
            PutScreen(cBody, color, aBody);
            PutScreen(cHead, color, aHead[(int)arrow]);
            PutScreen(cNone, color, aNone);
        }

        private void TurnTo(Arrow arrow)
        {
            if (this.arrow == arrow)
                return;
            this.arrow = arrow;
            step.x = 0;
            step.y = 0;
            switch (arrow)
            {
                case Arrow.L:
                    step.x = -1;
                    break;
                case Arrow.R:
                    step.x = +1;
                    break;
                case Arrow.U:
                    step.y = -1;
                    break;
                case Arrow.D:
                default:
                    step.y = +1;
                    break;
            }
        }

        private void Turn()
        {
            if (rnd.Next(10) > 0)
                if (IsEmpty(head + step))
                    return;
            for (int i = 0; i < 10; i++)
            {
                TurnTo((Arrow)rnd.Next(0, 4));
                if (IsEmpty(head + step))
                    return;
            }
        }

        public void Step()
        {
            Turn();
            Coord nextHead = head + step;
            if (IsEmpty(nextHead))
                body.Enqueue(nextHead);
            else
            {
                nextHead = head;
            }
            if (Screen(nextHead) == aHare)
            {
                grow++;
            }
            Coord none = new Coord(-1, -1);
            if (body.Count > 1)
            {
                if (grow > 0)
                    grow--;
                else
                    none = body.Dequeue();
            }
            ShowMe(nextHead, head, none);
            head = nextHead;
        }

        public static readonly char aHare = '"';
        public static readonly char[] aHead = { '<', '>', '^', 'v' };

        private static char[,] screen = new char[size.x, size.y];
        private static object block = new object();
        static Random rnd = new Random();
        private static int number = 0;

        public static void InitScreen()
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (x * y == 0 || x == size.x - 1 || y == size.y - 1)
                    {
                        PutScreen(new Coord(x, y), ConsoleColor.DarkBlue, aWall);
                    }
                    else
                    {
                        PutScreen(new Coord(x, y), ConsoleColor.DarkBlue, aNone);
                    }
                }
            }
        }
        private static void PutScreen(Coord coord, ConsoleColor color, char a)
        {
            lock (block)
            {
                if (!Onscreen(coord))
                    return;
                screen[coord.x, coord.y] = a;
                Console.ForegroundColor = color;
                Console.SetCursorPosition(coord.x, coord.y);
                Console.Write(a);
            }
        }
        public static void AddHare()
        {
            if (rnd.Next(10) > 0)
                return;
            Coord hare;
            int loop = 50;
            do
                hare = RandomCoord();
            while (!IsEmpty(hare) && --loop > 0);
            if (loop > 0)
                PutScreen(hare, ConsoleColor.Cyan, aHare);
        }
        public static Coord RandomCoord()
        {
            return new Coord(rnd.Next(1, size.x), rnd.Next(1, size.y));
        }
        public static bool IsEmpty(Coord coord)
        {
            char c = Screen(coord);
            return (c == aNone || c == aHare);
        }
        public static char Screen(Coord coord)
        {
            if (!Onscreen(coord))
            {
                return aWall;
            }
            return screen[coord.x, coord.y];
        }

        private static bool Onscreen(Coord coord)
        {
            return (coord.x >= 0 && coord.x < size.x &&
                    coord.y >= 0 && coord.y < size.y);
        }
        public enum Arrow
        {
            L,
            R,
            U,
            D
        };
        Coord head;
        Arrow arrow;
        Coord step;
        ConsoleColor color;
        Queue<Coord> body;
        bool dead;
        int grow;
        int nr;

        public static Python Create()
        {
            Coord start;
            int loop = 50;
            do
                start = RandomCoord();
            while (!IsEmpty(start) && --loop > 0);
            if (loop <= 0)
                return null;
            Python python = new Python(start);
            python.nr = number;
            number++;
            return python;
        }
        private Python(Coord start)
        {
            this.head = start;
            this.body = new Queue<Coord>();
            body.Enqueue(head);
            this.color = (ConsoleColor)rnd.Next(1, 15);
            TurnTo(Arrow.R);
            grow = 0;
            dead = false;

        }
        public void Run()
        {
            while (true)
            {

                try
                {
                    while (true)
                    {
                        Step();
                        AddHare();
                        Info();
                        //if (rnd.Next(100) == 0)
                        //    break;
                        Thread.Sleep(50);
                        PutScreen(head, color, aNone);
                        if (dead && body.Count <= 1)
                        {
                            return;
                        }
                    }
                }
                catch (ThreadAbortException ex)
                {
                    dead = true;
                    Thread.ResetAbort();
                }
            }
        }

        private void Info()
        {
            lock (block)
            {
                Console.SetCursorPosition(size.x + 2, nr);
                Console.ForegroundColor = color;
                Console.Write(nr + " " + body.Count + "#" + Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}
