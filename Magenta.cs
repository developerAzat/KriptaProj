using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KriptaProject
{
    class Magenta
    {
        Array

        public byte[] text; // дополненное до кратного 16 сообщение
                     // раундовый блок открытого текста, поделенный на левую и правую части:
        byte[] blockL = new byte[8];
        byte[] blockR = new byte[8];
        // ключ, поделенный на левую и правую части:
        byte[] keyL = new byte[8];
        byte[] keyR = new byte[8];

        byte[] EncodedData; // поле для хранения зашифрованного сообщения
        byte[] DecodedData; // поле для хранения расшифрованного зашифрованного сообщения

        byte alpha = 0x02;

        public Magenta() { }

        public Magenta(string Text, string Key) // конструктор
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

        private byte f(byte x)
        {
            if (x == 255)
                return 0;
            else
                return PowerGF256(alpha, x, 0x65);
        }

        private byte A(byte x, byte y)
        {
            return f((byte)(x ^ f(y)));
        }

        private byte[] PE(byte x, byte y)
        {
            byte[] result = new byte[2];
            result[0] = A(x, y);
            result[1] = A(y, x);

            return result;
        }

        private byte[] P(byte[] x)
        {
           
            byte[] res16 = new byte[16];
            /*int j = 0;
            for(int i=0;i<8;i++)
            {
                res16[j] = PE(x[i], x[i + 8])[0];
                j++;
                res16[j] = PE(x[i], x[i + 8])[1];
                j++;
            }*/

           for (int i = 0; i < 8; i++)
                res16[i] = PE(x[i], x[8 + i])[i % 2];
                
            return res16;
        }

        private byte[] T(byte[] x)
        {
            return P(P(P(P(x))));
        }

        private byte[] XE(byte[] x)
        {
            byte[] result = new byte[8];

            for(int i=0;i<8;i++)
            {
                result[i] = x[2 * i];
            }

            return result;
        }

        private byte[] XO(byte[] x)
        {
            byte[] result = new byte[8];

            for (int i = 0; i < 8; i++)
            {
                result[i] = x[2 * i + 1];
            }

            return result;
        }

        private byte[] C1(byte[] x)
        {
            return T(x);
        }

        private byte[] C2(byte[] x)
        {
            byte[] y = new byte[16];

            for (int i = 0; i < 8; i++)
            {
                y[i] = (byte)(x[i] ^ XE(C1(x))[i]);
            }
            for (int i = 8; i < 16; i++)
            {
                y[i] = (byte)(x[i] ^ XO(C1(x))[i - 8]);
            }

            return T(y);
        }

        private byte[] C3(byte[] x)
        {
            byte[] y = new byte[16];

            for (int i = 0; i < 8; i++)
            {
                y[i] = (byte)(x[i] ^ XE(C2(x))[i]);
            }
            for (int i = 8; i < 16; i++)
            {
                y[i] = (byte)(x[i] ^ XO(C2(x))[i - 8]);
            }

            return T(y);
        }

        private byte[] E(byte[] x)
        {
            return XE(C3(x));
        }

        private byte[,] F(byte[] xLeft, byte[] xRight, byte[] y8)
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
                z[1, i] = (byte)(xLeft[i] ^ E(xy)[i]);
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
                    res = F(blockL, blockR, keyL);
                else
                    res = F(blockL, blockR, keyR);

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
                    res = F(blockL, blockR, keyL);
                else
                    res = F(blockL, blockR, keyR);

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

        public static byte[] Encoding(byte[] blockText, byte[] blockKey)
        {
            Magenta a = new Magenta();
            byte[,] res = new byte[2, 8];
            // function V
            byte[] tmp = new byte[8];
            byte[] kL = new byte[8];
            byte[] kR = new byte[8];
            byte[] bL = new byte[8];
            byte[] bR = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                bL[i] = blockText[i];
                bR[i] = blockText[i + 8];
                kL[i] = blockKey[i];
                kR[i] = blockKey[i + 8];
            }
            for (int i = 0; i < 8; i++)
            {
                tmp[i] = bL[i];
                bL[i] = bR[i];
                bR[i] = tmp[i];
            }
            for (int j = 0; j < 6; j++)
            {
                if (j != 2 || j != 3)
                    res = a.F(bL, bR, kL);
                else
                    res = a.F(bL, bR, kR);
                for (int i = 0; i < 8; i++)
                {
                    bL[i] = res[0, i];
                    bR[i] = res[1, i];
                }
            }

            byte[] blockLR = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                blockLR[i] = bL[i];
                blockLR[i + 8] = bR[i];
            }
            return blockLR;
        }

    

    public static byte[] Encoding(string Text, string Key)
        {
            Magenta M = new Magenta(Text, Key);

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

        public static byte MultGF256(byte a, byte b,byte f)
        {
            byte t = 0, mask = 1;

            for (int i = 0; i < 8; i++)
            {
                if ((b & mask) != 0)
                    t = (byte)(t ^ a);
                if ((a & 128) == 128)
                    a = (byte)(a << 1);
                else
                    a = (byte)((a << 1) ^ f);
                mask = (byte)(mask << 1);
            }
            return t;
        }

        public static byte PowerGF256(byte a, byte b,byte f)
        {
            byte t = 1;

            while (b > 0)
            {
                if (b % 2 == 1)
                    t = MultGF256(a, b,f);
                a = MultGF256(a, a,f);
                b = (byte)(b >> 1);
            }
            return t;
        }

        public static string ToString(byte[] a)
        {
            return System.Text.Encoding.GetEncoding(866).GetString(a);
        }

        /*static void Main(string[] args)
        {
            string Text = "The quiet April day has sent me\n" +
                "What a strange missive.\n" +
                "You knew that passionately in me\n" +
                "The scary week is still alive.\n" +
                "I did not hear those ringing bells\n" +
                "That swam along in glazier clear.\n" +
                "For seven days sounded copper laugh\n" +
                "Or poured from eyes a silver tear.\n" +
                "And I, then having closed my face\n" +
                "As for eternal parting's moment,\n" +
                "Lay down and waited for her grace\n" +
                "That was not known yet as torment.\n";



             string Key = "если есть в кармане пачка сигарет\n";



             Console.WriteLine("Текст: {0} \n Ключ: {1}", Text, Key);

             

             byte[] encodedText = Encoding(Text, Key);
             Console.WriteLine("Зашифрованый текст: \n");

            for (int i=0;i<encodedText.Length;i++)
             {
                Console.Write(encodedText[i].ToString("x2"));
             }

            Console.Write("\n\n");

             string decodedText = ToString(Decoding(ToString(encodedText), Key));
             Console.WriteLine("Расшифрованый текст: {0} \n", decodedText);

             

             

            

            
            Console.ReadLine();
        }*/
    }
}
