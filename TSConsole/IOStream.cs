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
        public void PrintXml(string msg)
        {
            if (msg.Contains("<obj"))
            {
                try
                {
                    XDocument xdoc = XDocument.Parse(msg);
                    
                    var objs = from lv1 in xdoc.Descendants("obj")
                               select new
                               {
                                   color = lv1.Attribute("color").Value,
                                   text = lv1.Attribute("text").Value,
                                   line = lv1.Attribute("line").Value
                               };
                    foreach (var x in objs)
                    {
                        if (Enum.TryParse<ConsoleColor>(x.color, out ConsoleColor newcol))
                            Console.ForegroundColor = newcol;
                        if ((x.line == "True" || x.line == "true") ? true : false)
                            Console.WriteLine(x.text);
                        else
                            Console.Write(x.text);
                        Console.ResetColor();
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(msg);
                    Console.WriteLine(e);
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine(msg);
            }
        }
    }
}
