using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Exceptions
{
    internal interface IExceptionListener
    {
        string CurrentLine { get; }
        void Throw(ExceptionHandler ex);
        void Throw(string msg, ExceptionType type = ExceptionType.CompilerException, string lineref = "{0}");
        void ThrowSilent(ExceptionHandler ex, bool once = false);
        void SetCurrentLine(string s);
    }
    internal class ExceptionListener : IExceptionListener
    {
        private string _currentLine = "";
        public string CurrentLine { get { return _currentLine; } }
        private List<ExceptionHandler> _onceList = new List<ExceptionHandler>();
        public void Throw(ExceptionHandler ex)
        {
            TokenParser.Stop = true;
            if(Program.LogLevel == "warn" || Program.LogLevel == "error")
                IO.Output.Print($"\n[ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}\nCode Snippet:\n{ex.Snippet}", ConsoleColor.Red);
            throw new CompilerControledException();
        }
        public void Throw(string msg, ExceptionType type = ExceptionType.CompilerException, string lineref = "{0}")
        {
            var n = new ExceptionHandler(type, msg, lineref);
            Throw(n);
        }
        public void ThrowSilent(ExceptionHandler ex, bool once = false)
        {
            if ((!once) || (once && _onceList.FirstOrDefault(f => f.Message == ex.Message) == null))
            {
                if (Program.LogLevel == "warn")
                    IO.Output.Print($"[SILENT ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}", ConsoleColor.DarkYellow);
                if (once)
                {
                    _onceList.Add(ex);
                }
            }
        }
        public void SetCurrentLine(string s)
        {
            _currentLine = s;
        }
    }
    /// <summary>
    /// This is a ghetto way to silently stop the script from running when
    /// hitting an compiler controlled exception.
    /// </summary>
    internal class CompilerControledException : Exception
    {
    }
}
