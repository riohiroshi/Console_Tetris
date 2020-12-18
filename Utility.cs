using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Utility
    {
        static Random random = new Random();

        public static ConsoleColor GetRandomColor()
        {
            return (ConsoleColor)(random.Next(10, Enum.GetNames(typeof(ConsoleColor)).Length));
        }

        public static int GetRandomShape()
        {
            return random.Next(0, 7);
        }

        //public static void Center(string message)
        //{
        //    int screenWidth = Console.WindowWidth;

        //    char[] buffer = message.ToCharArray();
        //    string temp = "";

        //    for (int i = 0; i < buffer.Length; i++)
        //    {
        //        if (buffer[i] < 127)
        //        {
        //            temp += buffer[i].ToString() + " ";
        //        }
        //        else
        //        {
        //            temp += buffer[i].ToString();
        //        }
        //    }

        //    int lenTotal = 0;
        //    int n = message.Length;
        //    string strWord = "";
        //    int asc;
        //    for (int i = 0; i < n; i++)
        //    {
        //        strWord = message.Substring(i, 1);
        //        asc = Convert.ToChar(strWord);
        //        if (asc < 0 || asc > 127)
        //        {
        //            lenTotal = lenTotal + 2;
        //        }
        //        else
        //        {
        //            lenTotal = lenTotal + 1;
        //        }
        //    }



        //    int stringWidth = lenTotal;
        //    int spaces = (screenWidth / 2) + (stringWidth / 2);

        //    Console.WriteLine(temp.PadLeft(spaces));
        //}
    }
}