using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KriptaProject
{
    class HashFunc
    {
        public static string ToString(byte[] a)
        {
            return ASCIIEncoding.ASCII.GetString(a);
        }
        public static byte[] GetHash(byte[] text)
        {
           

            byte[] E = new byte[16];
            byte[] h = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                h[i] = 0x00;
            }
            byte[] M = new byte[16];
            byte[] A = new byte[16];
            for (int i = 0; i < text.Length; i = i + 16)
            {
                for (int j = 0; j < 16; j++)
                {
                    M[j] = text[i + j];
                    A[j] = (byte)(M[j] ^ h[j]);
                }
                E = Magenta.Encoding(A,M);
                for (int j = 0; j < 16; j++)
                {
                    h[j] = (byte)(E[j] ^ A[j]);
                }
            }
            return h;

        }

       /*public static void Main()
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

            Console.WriteLine("Text: " + Text);
            


            string space = "";

            if (Text.Length % 16 != 0)
                for (int i = 0; i < 16 - Text.Length % 16; i++)
                    space += " ";

            byte[] text = ASCIIEncoding.ASCII.GetBytes(Text + space);

           
            byte[] res = GetHash(text);

            Console.WriteLine("Text hex:");

            for(int i=0;i<text.Length;i++)
            {
                Console.Write(text[i].ToString("x2"));
            }
            Console.Write("\n\n\n");

            Console.WriteLine("HashCode: ");
            for(int i=0;i<16;i++)
            {
                Console.Write(res[i].ToString("x2"));
            }

            Console.ReadLine();
        }*/
    }
}
