using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.ParserManager;
using TastyScript.ParserManager.IOStream;

namespace TastyScript.TSConsole
{
    public class IOStream : IIOStream
    {
        public void Print(object o, bool line = true)
        {
            if (line)
                Console.WriteLine(o);
            else
                Console.Write(o);
        }
        public void Print(object o, ConsoleColor color, bool line = true)
        {
            Console.ForegroundColor = color;
            if (line)
                Console.WriteLine(o);
            else
                Console.Write(o);
            Console.ResetColor();
        }
        public string ReadLine()
        {
            return Console.ReadLine();
        }
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Console.ReadKey(intercept);
        }
        public void ChangeConsoleTitle(string append)
        {
            Console.Title = $"{Manager.Title} || {append}";
        }
    }
}
