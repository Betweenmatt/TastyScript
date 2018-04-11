using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using TastyScript.ParserManager;
using TastyScript.ParserManager.IOStream;

namespace TastyScript.TastyScript
{
    public class IOStream : IIOStream
    {
        public void Print(object o, bool line = true)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("obj");
            element.SetAttribute("color", "");
            element.SetAttribute("text", o.ToString());
            element.SetAttribute("line", line.ToString());
            Console.WriteLine(element.OuterXml);
        }
        public void Print(object o, ConsoleColor color, bool line = true)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("obj");
            element.SetAttribute("color", color.ToString());
            element.SetAttribute("text", o.ToString());
            element.SetAttribute("line", line.ToString());
            Console.WriteLine(element.OuterXml);
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
