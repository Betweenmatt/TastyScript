using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang;

namespace TastyScriptNPP
{
    class ExceptionListener : TastyScript.Lang.Exceptions.IExceptionListener
    {
        private string _currentLine = "";
        public string CurrentLine { get { return _currentLine; } }
        private List<ExceptionHandler> _onceList = new List<ExceptionHandler>();
        public void SetCurrentLine(string s)
        {
            _currentLine = s;
        }

        public void Throw(ExceptionHandler ex)
        {
            string msg = $"\n[ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}\nCode Snippet:\n{ex.Snippet}";

            IOStream.Instance.Print(msg, ConsoleColor.Red);

            throw new CompilerControledException();
        }

        public void Throw(string msg, ExceptionType type = ExceptionType.CompilerException, string lineref = "{0}")
        {
            var n = new ExceptionHandler(type, msg, lineref);
            Throw(n);
        }

        public void ThrowDebug(string msg)
        {
            IOStream.Instance.Print(msg, ConsoleColor.DarkYellow);
        }

        public void ThrowSilent(ExceptionHandler ex, bool once = false)
        {
            if ((!once) || (once && _onceList.FirstOrDefault(f => f.Message == ex.Message) == null))
            {
                string msg = $"[SILENT ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}";

                IOStream.Instance.Print(msg, ConsoleColor.DarkYellow);

                if (once)
                {
                    _onceList.Add(ex);
                }
            }
        }
    }
    public class CompilerControledException : Exception { }
}
