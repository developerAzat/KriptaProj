using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KriptaProject
{
    class CNFL:Magenta
    {
        public static byte[] CNFLEncoding(string Message,string key,byte s0)
        {
            Magenta magenta = new Magenta(Message, key);
            byte[] s = new byte[magenta.text.Length];
            s[0] = s0;
            for (int i = 1; i < magenta.text.Length; i++)
                s[i] = (byte)(s[i - 1] + 1);

            byte[] C = new byte[magenta.text.Length];
            byte[] Ci = new byte[16];
            byte[] Pi = new byte[16];
            byte[] ki;
            byte[] tmp = new byte[16];

            for(int i=0;i<magenta.text.Length;i+=16)
            {
                
                for (int j = 0; j < 16; j++)
                {
                    Pi[j] = magenta.text[j + i];
                    tmp[j] = s[j+i];
                }
                    ki = Encoding(ToString(tmp), key);
                Ci = Encoding(Pi, ki);

                for (int j = 0; j < 16; j++)
                    C[i + j] = Ci[j];
            }

            return C;

        }

        public static string CNFLDecoding(byte[] C,string key,byte s0)
        {
            byte[] s = new byte[C.Length];
            s[0] = s0;
            for (int i = 1; i < C.Length; i++)
                s[i] = (byte)(s[i - 1] + 1);

            byte[] Ci = new byte[16];
            byte[] Pi = new byte[16];
            byte[] P = new byte[C.Length];
            byte[] ki;
            byte[] tmp = new byte[16];

            for(int i=0;i<C.Length;i+=16)
            {
                for (int j = 0; j < 16; j++)
                {
                    Ci[j] = C[j + i];
                    tmp[j] = s[j + i];
                }
                ki = Encoding(ToString(tmp), key);
                Pi = Decoding(ToString(Ci), ToString(ki));

                for (int j = 0; j < 16; j++)
                    P[i + j] = Pi[j];
            }

            return ToString(P);

        } 

      /* public static void Main()
        {
            String Message = "The quiet April day has sent me\n" +
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

            string key = "AzatMSalahutdinov";

            RC4 rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes("getbackmotherfucker"));

            byte s0 = rc4.Rand();

            Console.WriteLine("Message : " + Message);

            Console.WriteLine("Key: " + key);

            Console.WriteLine("Шифрование производится в режиме CNFL");
            Console.WriteLine("****************************************************************************************************");

            byte[] encodedText = CNFLEncoding(Message, key, s0);

            Console.WriteLine("Encoding Message: ");

            for (int i = 0; i < encodedText.Length; i++)
                Console.Write(encodedText[i].ToString("x2") + " ");
            Console.Write("\n");

            Console.WriteLine("Decoding Message: \n" + CNFLDecoding(encodedText, key, s0));
            Console.ReadLine();
        }*/
    }
}
