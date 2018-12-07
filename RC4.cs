using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KriptaProject
{
    class RC4
    {
        const int N = 256;

        byte[] S = new byte[N];

        int iRC4, jRC4, j;

        byte[] K = new byte[N];
        public RC4(byte[] mk)
        {
            Init(mk);

        }

        private void Swap(ref byte a,ref byte b)
        {
            byte temp = a;
            a = b;
            b = temp;
        }

        public void Init(byte[] mk)
        {
            for (int i = 0; i < N; i++)
            {
                S[i] = (byte)i;
            }

            for (int i = 0; i < N; ++i)
            {
                K[i] = mk[i % mk.Length];
            }

            iRC4 = 0; jRC4 = 0; j = 0;

            for (int i = 0; i < N; ++i)
            {
                j = (j + S[i] + K[i]) % 256;
                byte tmp = S[i];
                S[i] = S[j];
                S[j] = tmp;
            }
        }

        public byte Rand()
        {
            iRC4 = (iRC4 + 1) % N;
            jRC4 = (jRC4 + S[iRC4]) % N;
            Swap(ref S[iRC4],ref S[jRC4]);
            return S[(S[iRC4] + S[jRC4]) % N];
        }

        public byte RC4M(int k)
        {
            while (k > 256)
            {
                k -= N;
            }
            int c;
            int y = N - (N % k);
            while (true)
            {
                c = Rand();

                if (c < y)
                {
                    c = c % k;
                    return (byte)c;
                }
            }
        }

        public void Transp(int t) {
            byte[] s = new byte[t];
            byte[] r = new byte[t];

            int[] freq = new int[t];

            for(int i=0;i<t;++i)
            {
                s[i] = (byte)i;
            }

            byte c = RC4M(t);
            r[0] = s[c];
            Swap(ref s[c], ref s[t - 1]);
            freq[r[0]]++;

            for (int i=1;i<t;++i)
            {
                c = RC4M(t - i);
                r[i] = s[c];
                Swap(ref s[c], ref s[t - i - 1]);
                freq[r[i]]++;
            }

            for(int i=0;i<t;++i)
            {
                Console.Write(r[i]);
                Console.Write(' ');
            }

           
        }

       /*public static void Main()
        {
            Console.WriteLine("Введите ключ: ");
            byte[] mk = ASCIIEncoding.ASCII.GetBytes(Console.ReadLine()); 
            RC4 rc4 = new RC4(mk);
            Console.WriteLine();
            Console.WriteLine("Псевдослучайно сгенерированное число: " + rc4.Rand());
            Console.WriteLine();
            Console.Write("Введите диапазон от 0 до n-1, где n: ");
            int n = Convert.ToInt32(Console.ReadLine());
            int m = n - 1;
            Console.WriteLine("Случайное число в диапазоне от 0 до " + m + ": " + rc4.RC4M(n));
            Console.WriteLine();
            Console.WriteLine("Случайная перестановка Z[100]: ");
            rc4.Transp(100);
            Console.ReadLine();
        }*/
    }
}
