using System;
using System.Collections.Generic;
using System.Linq;

namespace TastyScript.Lang.Exceptions
{
    public interface IExceptionListener
    {
        string CurrentLine { get; }
        void Throw(ExceptionHandler ex);
        void Throw(string msg, ExceptionType type = ExceptionType.CompilerException, string lineref = "{0}");
        void ThrowSilent(ExceptionHandler ex, bool once = false);
        /// <summary>
        /// Prints the debug messagage in the console as DarkYellow color
        /// and logs the message in debug.log
        /// </summary>
        /// <param name="msg"></param>
        void ThrowDebug(string msg);
        void SetCurrentLine(string s);
        TryCatchStack TryCatchEventStack { get; }
    }
    public class ExceptionListener : IExceptionListener
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _currentLine = "";
        public string CurrentLine { get { return _currentLine; } }
        private List<ExceptionHandler> _onceList = new List<ExceptionHandler>();
        public TryCatchStack TryCatchEventStack { get; }
        //i think this fixes my stupid mistake that was breaking as an assembly
        public static bool stupidFix;

        public ExceptionListener()
        {
            TryCatchEventStack = new TryCatchStack();
        }

        public static void LogDebug(string message)
        {
            if (!stupidFix)
                log.Debug(message);
        }
        public static void LogInfo(string message)
        {
            if (!stupidFix)
                log.Info(message);
        }
        public static void LogWarn(string message)
        {
            if (!stupidFix)
                log.Warn(message);
        }
        public static void LogError(string message)
        {
            if (!stupidFix)
                log.Error(message);
        }
        public static void LogThrow(string message, Exception e)
        {
            if (!stupidFix)
                log.Fatal(message, e);
            else
                Main.IO.Print(message + e, ConsoleColor.DarkRed);
        }
        public void ThrowDebug(string msg)
        {
            LogDebug(msg);
            Main.IO.Print(msg, ConsoleColor.DarkYellow);
        }
        public void Throw(ExceptionHandler ex)
        {
            if (TryCatchEventStack == null || TryCatchEventStack.Last() == null)
            {
                TokenParser.Stop = true;
                string msg = $"\n[ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}\nCode Snippet:\n{ex.Snippet}";
                LogError(msg);
                if (Settings.LogLevel == "warn" || Settings.LogLevel == "error" || Settings.LogLevel == "throw")
                {
                    Main.IO.Print(msg, ConsoleColor.Red);
                }
                throw new CompilerControledException();
            }
            else
            {
                TryCatchEventStack.Last().TriggerCatchEvent(ex);
            }
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
                    Main.IO.Print(msg, ConsoleColor.DarkYellow);
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
    public class CompilerControledException : Exception
    {
    }
    public class TryCatchEvent
    {
        internal IBaseFunction TryBlock { get; }
        internal IBaseFunction CatchBlock { get; }
        internal TryCatchEvent(IBaseFunction tryblock, IBaseFunction catchblock)
        {
            TryBlock = tryblock;
            CatchBlock = catchblock;
        }
        public void TriggerCatchEvent(ExceptionHandler ex)
        {
            Compiler.ExceptionListener.TryCatchEventStack.RemoveLast();
            TryBlock.ReturnFlag = true;
            string line = System.Text.RegularExpressions.Regex.Escape(ex.Line);
            string[] arg = new string[] { "[" + ex.Type.ToString().CleanString() + "," + ex.Message.CleanString() + "," + line.CleanString() + "," + ex.Snippet.CleanString() + "]" };
            CatchBlock.TryParse(new Tokens.TFunction(CatchBlock, null, arg, TryBlock));
        }
    }
    public class TryCatchStack
    {
        private List<TryCatchEvent> _tlist;
        public TryCatchStack()
        {
            _tlist = new List<TryCatchEvent>();
        }
        public void Add(TryCatchEvent item)
        {
            _tlist.Add(item);
        }
        public void RemoveLast()
        {
            _tlist.RemoveAt(_tlist.Count - 1);
        }
        public TryCatchEvent Last()
        {
            return _tlist.LastOrDefault();
        }
    }
}
