using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.IOStream
{
    public interface IIOStream
    {
        void Print(object msg, bool line);
        void Print(object msg, ConsoleColor color, bool line);
        string ReadLine();
        ConsoleKeyInfo ReadKey(bool intercept);
        void ChangeConsoleTitle(string append);
    }
}
