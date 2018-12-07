using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics; 

namespace KriptaProject
{
    class RC41

    {

        const int M = 256;

        byte[] S = new byte[M];

        int x = 0;

        int y = 0;

        public RC41(byte[] key)

        {

            Init(key);

        }

      

        private void Init(byte[] key)

        {

            for (int i = 0; i < M; i++)

            {

                S[i] = (byte)i;

            }

            for (int i = 0, j = 0; i < M; i++)

            {

                j = (j + S[i] + key[i]) % M;

                byte tmp = S[i];

                S[i] = S[j];

                S[j] = tmp;

            }

        }

        private byte KeyItem()

        {

            x = (x + 1) % M;

            y = (y + S[x]) % M;

            byte tmp = S[x];

            S[x] = S[y];

            S[y] = tmp;

            return S[(S[x] + S[y]) % M];

        }

        public int Rand(int n)

        {

            int b;

            while (true)

            {

                b = KeyItem();

                if (b < n)

                    return b;

            }

        }

        public static string InputKey()

        {

            Console.WriteLine("Введите ключ: ");

            string InputKey = Console.ReadLine();

            string StrKey = "";

            while (StrKey.Length < M)

            {

                StrKey += InputKey;

            }

            StrKey = StrKey.Substring(0, M);

            return StrKey;

        }

        /*static void Main(string[] args)

        {



            byte[] Key = ASCIIEncoding.ASCII.GetBytes(InputKey());

             RC41 generator = new RC41(Key);

             Console.WriteLine("Введите диапазон [0, n-1], n: ");

             int n = Convert.ToInt32(Console.ReadLine());

             int[] Frequency = new int[n];

             for (int i = 0; i < 1000; i++)

             {

                 int RandNum = generator.Rand(n);

                 Console.WriteLine("Рандомное число №{0}: " + RandNum, i + 1);

                 Frequency[RandNum]++;

             }

             Console.WriteLine("Частотный анализ: ");

             for (int i = 0; i < n; i++)

             {

                 Console.WriteLine(i + ": " + Frequency[i] + " time");

             }

             Console.ReadLine();

            byte[] mk = ASCIIEncoding.ASCII.GetBytes(Console.ReadLine());
            RC4 kek = new RC4(mk);

            Console.WriteLine(kek.Rand());

            kek.Transp(100);

            Console.ReadLine();
        }
    */
    }



namespace Maenta
    {
        class Mgnta
        {
            byte[] text; // дополненное до кратного 16 сообщение
                         // раундовый блок открытого текста, поделенный на левую и правую части:
            byte[] blockL = new byte[8];
            byte[] blockR = new byte[8];
            // ключ, поделенный на левую и правую части:
            byte[] keyL = new byte[8];
            byte[] keyR = new byte[8];

            byte[] EncodedData; // поле для хранения зашифрованного сообщения
            byte[] DecodedData; // поле для хранения расшифрованного зашифрованного сообщения

            byte alpha = 0x02; // альфа

            public Mgnta(string Text, string Key) // конструктор
            {
                string space = "";

                if (Text.Length % 16 != 0)
                    for (int i = 0; i < 16 - Text.Length % 16; i++)
                        space += " ";

                text = ASCIIEncoding.ASCII.GetBytes(Text + space);

                for (int i = 0; i < 8; i++)
                {
                    keyL[i] = ASCIIEncoding.ASCII.GetBytes(Key.Substring(0, 8))[i];
                    keyR[i] = ASCIIEncoding.ASCII.GetBytes(Key.Substring(8, 8))[i];
                }
            }

            private byte funcf(byte x)
            {
                if (x == 255) return 0;

                return Power(alpha, x);
            }

            private byte funcA(byte x, byte y)
            {
                return funcf((byte)(x ^ funcf(y)));
            }

            private byte[] funcPE(byte x, byte y)
            {
                byte[] res = new byte[2];

                res[0] = funcA(x, y);
                res[1] = funcA(y, x);

                return res;
            }



            private byte[] funcTT(byte[] x16) // function П
            {
                byte[] res16 = new byte[16];

                for (int i = 0; i < 8; i++)
                    res16[i] = funcPE(x16[(15 + i) % 16], x16[(8 + i)])[i % 2];

                return res16;
            }

            private byte[] funcT(byte[] x16)
            {
                byte[] res = new byte[16];

                res = funcTT(funcTT(funcTT(funcTT(x16))));

                return res;
            }

            private byte[] funcXE(byte[] x16)
            {
                byte[] res = new byte[8];

                for (int i = 0; i < 8; i++)
                    res[i] = x16[2 * i];

                return res;
            }

            private byte[] funcXO(byte[] x16)
            {
                byte[] res = new byte[8];

                for (int i = 0; i < 8; i++)
                    res[i] = x16[1 + 2 * i];

                return res;
            }

            private byte[] funcC1(byte[] x16)
            {
                return funcT(x16);
            }

            private byte[] funcC2(byte[] x16)
            {
                byte[] y16 = new byte[16];

                for (int i = 0; i < 8; i++)
                {
                    y16[i] = (byte)(x16[i] ^ funcXE(funcC1(x16))[i]);
                }
                for (int i = 8; i < 16; i++)
                {
                    y16[i] = (byte)(x16[i] ^ funcXO(funcC1(x16))[i - 8]);
                }

                return y16;
            }






            private byte[] funcC3(byte[] x16)
            {
                byte[] y16 = new byte[16];

                for (int i = 0; i < 8; i++)
                {
                    y16[i] = (byte)(x16[i] ^ funcXE(funcC2(x16))[i]);
                }
                for (int i = 8; i < 16; i++)
                {
                    y16[i] = (byte)(x16[i] ^ funcXO(funcC2(x16))[i - 8]);
                }

                return y16;
            }

            private byte[] funcE(byte[] x16)
            {
                return funcXE(funcC3(x16));
            }

            //Сеть Фейстеля

            private byte[,] funcF(byte[] xLeft, byte[] xRight, byte[] y8)
            {
                byte[,] z = new byte[2, 8];
                byte[] xy = new byte[16];

                for (int i = 0; i < 8; i++)
                {
                    xy[i] = xRight[i];
                    xy[i + 8] = y8[i];
                }

                for (int i = 0; i < 8; i++)
                {
                    z[0, i] = xRight[i];
                    z[1, i] = (byte)(xLeft[i] ^ funcE(xy)[i]);
                }

                return z;
            }

            private void encoding()
            {
                byte[,] res = new byte[2, 8];
                // function V
                byte[] temp = blockL;
                blockL = blockR;
                blockR = temp;

                for (int j = 0; j < 6; j++)
                {
                    if (j != 2 || j != 3)
                        res = funcF(blockL, blockR, keyL);
                    else
                        res = funcF(blockL, blockR, keyR);

                    for (int i = 0; i < 8; i++)
                    {
                        blockL[i] = res[0, i];
                        blockR[i] = res[1, i];
                    }
                }
                // запись раундового шифр-тескта в общий блок
                if (EncodedData != null)
                    EncodedData = EncodedData.Concat(blockL).ToArray();
                else
                    EncodedData = blockL;
                EncodedData = EncodedData.Concat(blockR).ToArray();
            }

            private void decoding()
            {
                byte[,] res = new byte[2, 8];
                // function V
                byte[] temp = blockL;
                blockL = blockR;
                blockR = temp;

                for (int j = 0; j < 6; j++)
                {
                    if (j != 2 || j != 3)
                        res = funcF(blockL, blockR, keyL);
                    else
                        res = funcF(blockL, blockR, keyR);

                    for (int i = 0; i < 8; i++)
                    {
                        blockL[i] = res[0, i];
                        blockR[i] = res[1, i];
                    }
                }
                // запись раундового расшифрованного тескта в общий блок
                if (DecodedData != null)
                    DecodedData = DecodedData.Concat(blockL).ToArray();
                else
                    DecodedData = blockL;
                DecodedData = DecodedData.Concat(blockR).ToArray();

            }

            public static byte[] Encoding(string Text, string Key)
            {
                Mgnta M = new Mgnta(Text, Key);

                // разделение каждого 16-битного блока текста на левый и правый
                for (int j = 0; j < M.text.Length; j += 16)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        M.blockL[i] = M.text[i + j];
                        M.blockR[i] = M.text[i + j + 8];
                    }
                    M.encoding();
                }

                return M.EncodedData;
            }

            public static byte[] Decoding(string CipherText, string Key)
            {
                //Magenta инволютивна:
                return Encoding(CipherText, Key);
            }

            //Операции в поле 256



            public static byte Mult(byte a, byte b)
            {
                byte t = 0, mask = 1, px = 101;

                for (int i = 0; i < 8; i++)
                {
                    if ((b & mask) != 0)
                        t = (byte)(t ^ a);
                    if ((a & 128) == 128)
                        a = (byte)(a << 1);
                    else
                        a = (byte)((a << 1) ^ px);
                    mask = (byte)(mask << 1);
                }

                return t;
            }

            public static byte Power(byte a, byte b)
            {
                byte t = 1;

                while (b > 0)
                {
                    if (b % 2 == 1)
                        t = Mult(a, b);
                    a = Mult(a, a);
                    b = (byte)(b >> 1);
                }

                return t;
            }

            public static string ToString(byte[] a)
            {
                return ASCIIEncoding.ASCII.GetString(a);
            }

           /* static void Main(string[] args)
            {
                string Text = " The rain and the wind and the murk\n" +
                                 " Reign over cold desert of fall, \n" +
                                 " Here, life's interrupted till spring; \n" +
                                 " Till the spring, gardens barren and tall. \n" +
                                 " I'm alone in my house, it's dim\n" +
                                 " At the easel, and drafts through the rims. \n";

                

                string Key = "0123456701234567\n";

                

                Console.WriteLine("Текст: {0} \n Ключ: {1}", Text, Key);

                DateTime t1, t2;
                t1 = DateTime.Now;

                string encodedText = ToString(Encoding(Text, Key));
                Console.WriteLine("Зашифрованый текст: {0} \n", encodedText);

                string decodedText = ToString(Decoding(encodedText, Key));
                Console.WriteLine("Расшифрованый текст: {0} \n", decodedText);

                t2 = DateTime.Now;

                Console.WriteLine("Время: {0}", (t2 - t1).TotalMilliseconds);

                Console.ReadLine();
            }*/
        }
    }

    class Podpis
    {
        public Podpis() { }
        public byte[] Signature(byte[] textAugmented)

        {
            try
            {
                

                

                byte[] digSign = new byte[48];

                

                byte[] hash = HashFunc.GetHash(textAugmented).Concat(new byte[] { 0 }).ToArray();

                

                var rc4 = new Random(Convert.ToInt32(hash[1]));

                BigInteger pMinusOneDivQ = (P - 1) / Q;

                
                BigInteger gamma = Generate(P, rc4, 32);

                

                BigInteger G = BigInteger.ModPow(gamma, pMinusOneDivQ, P);

                

                BigInteger X = Generate(Q, rc4, 16);

                

                BigInteger Y = BigInteger.ModPow(G, X, P);

                

                BigInteger K = Generate(Q, rc4, 16);

                

                BigInteger H = new BigInteger(hash);

                

                BigInteger R = BigInteger.ModPow(G, K, P);

                

                BigInteger Ro = R % Q;

                

                BigInteger S1 = ((((Ro) * (K)) - ((H) * (X))) % Q) + Q;
                BigInteger S = S1 % Q;

                

                BigInteger check1 = BigInteger.ModPow(R, Ro, P);
                BigInteger check2 = ((BigInteger.ModPow(G, S, P)) * (BigInteger.ModPow(Y, H, P))) % P;
                Console.WriteLine("Подпись верна - " + (check1 == check2));

                

                byte[] rr = R.ToByteArray();
                byte[] ss = S.ToByteArray();
                R.ToByteArray().CopyTo(digSign, 0);
                Array.Copy(S.ToByteArray(), 0, digSign, 32, 16);
                return digSign;
            }
            catch (Exception ex)
            {
                Console.WriteLine("При наложении подписи произошла ошибка!\n" + ex.ToString());
                return null;
            }

        }



        public static BigInteger Generate(BigInteger n, Random rc4, int qbyte)

        {
            BigInteger maxBigInteger = n - BigInteger.One;
            byte[] arrayB = new byte[qbyte];
            Array.Copy(maxBigInteger.ToByteArray(), 0, arrayB, 0, qbyte);
            byte[] arr = new byte[arrayB.Length];
            bool flag = true;
            for (int i = arrayB.Length - 1; i >= 0; i--)
            {
                byte temp;
                if (flag)
                {
                    temp = (byte)rc4.Next(arrayB[i]);
                    flag = arrayB[i] == temp;
                }
                else
                {
                    temp = (byte)rc4.Next(byte.MaxValue);
                }
                arr[i] = temp;
            }
            arr[arrayB.Length - 1] = 0x00;
            return new BigInteger(arr);
        }

        
        BigInteger Q = BigInteger.Parse("286684287275266243736166399217868759773");
      
        BigInteger P = BigInteger.Parse("97553607833069877720851753590758150859523200990282982100522379573651322066419");


        /*public static void Main()
        {
            String Text = "This Message about big numbers!)";
            Podpis p = new Podpis();
            string space = "";

            if (Text.Length % 16 != 0)
                for (int i = 0; i < 16 - Text.Length % 16; i++)
                    space += " ";

            byte[] text = ASCIIEncoding.ASCII.GetBytes(Text + space);

            byte[] res = p.Signature(text);

            Console.WriteLine("Message: " + Text);

            Console.WriteLine("Message Hex: ");

            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(text[i].ToString("x2"));
            }
            Console.Write("\n\n");

            Console.WriteLine("Signature: ");
            for (int i = 0; i < 48; i++)
            {
                Console.Write(res[i].ToString("x2"));
            }
            Console.Write("\n");
            
            Console.ReadLine();
        }*/
    }
}
