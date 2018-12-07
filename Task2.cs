using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KriptaProject
{
    class NOD
    {
        int x, y;
        public NOD(int a, int b)
        {
            this.x = a;
            this.y = b;
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public  int Result()
        {
            int a = Math.Abs(x);
            int b = Math.Abs(y);
            while(a>0 & b>0)
            {
                if (a > b)
                    a = a % b;
                else
                    b = b % a;
            }
            return a + b;
        }

       /*public static void Main()
        {
            Console.WriteLine("Введите два числа: ");

           int a = Convert.ToInt32(Console.ReadLine());
            int b = Convert.ToInt32(Console.ReadLine());

            NOD nod = new NOD(a, b);

            Console.Write("НОД {0} и {1}: {2} ",nod.X,nod.Y,nod.Result());
            Console.ReadLine();
            Console.ReadLine();
        }*/

        
    }   

    class POW
    {
        int a;
        int n;
        long x;

        public POW(int a,int n, long x)
        {
            this.a = a;
            this.n = n;
            this.x = x;
        }

        public int Slow()
        {
            int y = 1;

            for(int i=0;i<x;i++)
            {
                y = (y * a) % n;
            }

            return y;
        }

        public int Fast()
        {
            long power = x;
            int val = a;
            int y = 1;
            while(power>0)
            {
                if(power % 2 !=0)
                {
                    y = (y * val) % n;
                }
                val = (val * val) % n;
                power = power / 2;
            }

            return y;
        }

       public static void Main()
        {
            Console.Write("Введите число: ");
            int a = Convert.ToInt32(Console.ReadLine());
            Console.Write("Введите степень: ");
            long x = Convert.ToInt64(Console.ReadLine());
            Console.Write("Введите mod: ");
            int n = Convert.ToInt32(Console.ReadLine());

            POW pow = new POW(a, n, x);
            int slow, fast;

            DateTime t1 = DateTime.Now;
            slow = pow.Slow();
            DateTime t2 = DateTime.Now;
            fast = pow.Fast();
            DateTime t3 = DateTime.Now;

            Console.WriteLine("{0} в степени {1} по модулю {2} = {3}[медленная]", a,x,n, slow);
            Console.WriteLine("{0} в степени {1} по модулю {2} = {3}[быстрая]", a, x, n, fast);

            Console.WriteLine("Время медленного возведения: {0}мс", (t2 - t1).Milliseconds);
            Console.WriteLine("Время быстрого возведения: {0}мс", (t3 - t2).Milliseconds);

            Console.ReadLine();


        }
    }

    class MultBack
    {
        static int[] Mult(int a,int n,int y=0,int x=1)
        {
            int d = a, m = n;

            int r = m % d;
            int q, z;

            while (r > 0)
            {
                q = m / d;
                z = (y + (n - ((q * x) % n))) % n;
                m = d;
                d = r;
                y = x;
                x = z;
                r = m % d;
            }

            int[] d_x = { d, x };

            return d_x;
        }

        public static long Teorem(long[] x,long[] y)
        {
            bool b = true;
            for(int i=0;i<y.Length&&b;i++)
                for(int j=i+1;j<y.Length&&b;j++)
                {
                    NOD nod = new NOD((int)y[i], (int)y[j]);
                    if (nod.Result() != 1)
                        b = false;
                }
            if (!b)
            {
                Console.WriteLine("Не взаимно простые");
                return 0;
            }
            long d, a = 1, k=0;
            for (int i = 0; i < y.Length; i++)
                a = a * y[i];
            long[] q = new long[y.Length];

            for (int i = 0; i < y.Length; i++)
                q[i] = a / y[i];

            long[] g = new long[y.Length];

            q[0] = Mult((int)q[0], (int)y[0], (int)g[0])[0];
            d = Mult((int)q[0], (int)y[0], (int)g[0])[1];

            for (int i = 1; i < y.Length; i++)
            {
                q[i] = Mult((int)q[i], (int)y[i], (int)g[i], (int)d)[0];
                d = Mult((int)q[i], (int)y[i], (int)g[i], (int)d)[1];
            }

            for (int i = 0; i < y.Length; i++)
                k += x[i] * q[i] * g[i];

            k = k % a;

            return k;

        }

       /* public static void Main()
        {
            Console.Write("Пусть a = ");
            int a = Convert.ToInt32(Console.ReadLine());

            Console.Write("и n = ");
            int n = Convert.ToInt32(Console.ReadLine());

            int d = Mult(a, n)[0];
            int x = Mult(a, n)[1];

            Console.WriteLine("a * x = d (mod n), ");
            Console.WriteLine("то есть {0} * {1} = {2} (mod {3})", a, x, d, n);

            if (d == 1)
                Console.WriteLine("Так как d == 1, то x = {0} - мульт. обратный к {1} в поле {2}", x, a, n);
            else
                Console.WriteLine("Так как d != 1, то a = {0} является делителем нуля в поле {1}", a, n);

            Console.ReadLine();
        }*/

        
    }

    
}
