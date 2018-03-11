using System;
using System.Collections.Generic;
using System.Linq;

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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _currentLine = "";
        public string CurrentLine { get { return _currentLine; } }
        private List<ExceptionHandler> _onceList = new List<ExceptionHandler>();
        public static void LogInfo(string message)
        {
            log.Info(message);
        }
        public static void LogWarn(string message)
        {
            log.Warn(message);
        }
        public static void LogError(string message)
        {
            log.Error(message);
        }
        public static void LogThrow(string message, Exception e)
        {
            log.Fatal(message, e);
        }
        public void Throw(ExceptionHandler ex)
        {
            TokenParser.Stop = true;
            string msg = $"\n[ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}\nCode Snippet:\n{ex.Snippet}";
            LogError(msg);
            if (Settings.LogLevel == "warn" || Settings.LogLevel == "error" || Settings.LogLevel == "throw")
            {
                IO.Output.Print(msg, ConsoleColor.Red);
            }
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
                string msg = $"[SILENT ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}";
                LogWarn(msg);
                if (Settings.LogLevel == "warn")
                {
                    IO.Output.Print(msg, ConsoleColor.DarkYellow);
                }
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
