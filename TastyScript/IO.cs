using System;

namespace TastyScript
{
    public class IO
    {
        public class Output
        {
            public static void Print(object o, bool line = true)
            {
                if (line)
                    Console.WriteLine(o);
                else
                    Console.Write(o);
            }
            public static void Print(object o, ConsoleColor color, bool line = true)
            {
                Console.ForegroundColor = color;
                if (line)
                    Console.WriteLine(o);
                else
                    Console.Write(o);
                Console.ResetColor();
            }
        }
        public class Input
        {
            public static string ReadLine()
            {
                return Console.ReadLine();
            }
        }
    }
}
