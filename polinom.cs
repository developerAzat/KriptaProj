using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KriptaProject
{
    class Polinom
    {
        private byte coef;
       

        public Polinom()
        {
            coef = 0;
        }
        public Polinom(byte Coef)
        {
            coef = Coef;
        }



        public static Polinom MultGF256(Polinom a,Polinom b,byte f)
        {
            byte t = 0, mask = 1;
            byte ac = a.coef, bc = b.coef;

            for (int i = 0; i < 8; i++)
            {
                if ((bc & mask) != 0)
                    t = (byte)(t ^ ac);
                if ((ac & 128) != 0)
                    ac = (byte)(ac << 1);
                else
                    ac = (byte)((ac << 1) ^ f);
                mask = (byte)(mask << 1);
            }

            return new Polinom(t);
        }


        public static Polinom PowerGF256(Polinom a, Polinom b,byte f)
        {
            byte c = 1;
            byte bc = b.coef;

            while (bc > 0)
            {
                if (bc % 2 == 1)
                    c = MultGF256(a,b,f).coef;
                a = MultGF256(a,a,f);
                bc = (byte)(bc >> 1);
            }

            return new Polinom(c);
        }


        public override string ToString()
        {
            string s = "";
            byte c = coef;

            for (int i = 0; i < 8; i++)
            {
                string t = "";


                if (c % 2 == 1)
                {

                    t += " + ";
                    if (i == 0)
                        t += "1";
                    else
                        t += "x^" + i;
                }
                t += s;
                s = null;
                s = t;

                c = (byte)(c >> 1);
            }
            s = s.Substring(3);

            return s;
        }

       /*static void Main(string[] args)
        {
             Console.WriteLine("f(x) = x^8 + x^4 + x^3 + x^2 + 1\n - неприводимый многочлен");

             Console.Write("Введите первое число для полинома:");
             byte a = Convert.ToByte(Console.ReadLine());
             Console.Write("Введите второе число для полинома:");
             byte b = Convert.ToByte(Console.ReadLine());


             Polinom A = new Polinom(a);
             Polinom B = new Polinom(b);
             const byte irreducible = (byte)0x1d; // x^8 + x^4 + x^3 + x^2 + 1 

         Console.WriteLine("Полином I - A: {0} ({1})", A.ToString(), a);
             Console.WriteLine("Полином II - B: {0} ({1}) \n", B.ToString(), b);

             Console.WriteLine("A * B = {0} ({1})", MultGF256(A,B,irreducible).ToString(), MultGF256(A, B, irreducible).coef);
             Console.WriteLine("A ^ B = {0} ({1})", PowerGF256(A, B, irreducible).ToString(), PowerGF256(A, B, irreducible).coef);

            

            Console.ReadLine();
        }*/
    }

}
