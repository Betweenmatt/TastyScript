using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.IOStream
{
    public interface IIOStream
    {
        void Print(string msg, bool line);
        void Print(string msg, ConsoleColor color, bool line);
        string ReadLine();
        void ChangeConsoleTitle(string append);
    }
}
