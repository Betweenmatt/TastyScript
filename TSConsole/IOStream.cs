using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TastyScript.ParserManager;
using TastyScript.ParserManager.IOStream;

namespace TastyScript.TSConsole
{
    internal class IOStream : IIOStream
    {
        public void Print(object o, bool line = true, string id = "")
        {
            if (line)
                Console.WriteLine(o);
            else
                Console.Write(o);
        }
        public void Print(object o, ConsoleColor color, bool line = true, string id = "")
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
        public void PrintXml(XmlStreamObj msg)
        {
            Console.ForegroundColor = msg.Color;
            if (msg.Line)
                Console.WriteLine(msg.Text);
            else
                Console.Write(msg.Text);
            Console.ResetColor();
        }
    }
}
