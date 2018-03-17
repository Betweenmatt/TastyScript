using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript
{
    public interface IIOStream
    {
        void Print(object o, bool line = true);
        void Print(object o, ConsoleColor color, bool line = true);
        string Read();
        void ChangeConsoleTitle(string append);
        ConsoleKeyInfo ReadKey(bool intercept);
    }
}
