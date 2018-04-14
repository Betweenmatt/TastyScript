using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.ParserManager.ExceptionHandler
{
    public class ExceptionHandler : IExceptionHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _currentLine = "";
        public string CurrentLine { get { return _currentLine; } }
        private List<ExceptionObject> _onceList = new List<ExceptionObject>();
        public TryCatchStack TryCatchEventStack { get; }
        //i think this fixes my stupid mistake that was breaking as an assembly
        public static bool stupidFix;

        public ExceptionHandler()
        {
            TryCatchEventStack = new TryCatchStack();
        }

        public void LogDebug(string message)
        {
            if (!stupidFix)
                log.Debug(message);
        }
        public void LogInfo(string message)
        {
            if (!stupidFix)
                log.Info(message);
        }
        public void LogWarn(string message)
        {
            if (!stupidFix)
                log.Warn(message);
        }
        public void LogError(string message)
        {
            if (!stupidFix)
                log.Error(message);
        }
        public void LogThrow(string message, Exception e)
        {
            if (!stupidFix)
                log.Fatal(message, e);
            else
                Manager.Print(message + e, ConsoleColor.DarkRed);
        }
        public void ThrowDebug(string msg)
        {
            LogDebug(msg);
            Manager.Print(msg, ConsoleColor.DarkYellow);
        }
        public void Throw(string message) => Throw(message, ExceptionType.CompilerException);
        public void Throw(string message, ExceptionType type = ExceptionType.CompilerException)
        {
            var ex = new ExceptionObject(message, type);
            if (TryCatchEventStack == null || TryCatchEventStack.Last() == null)
            {
                Manager.IsScriptStopping = true;
                string msg = $"\n[ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}\nCode Snippet:\n{ex.Snippet}";
                LogError(msg);
                if (Settings.LogLevel == "warn" || Settings.LogLevel == "error" || Settings.LogLevel == "throw")
                {
                    Manager.Print(msg, ConsoleColor.Red);
                }
                throw new CompilerControlledException();
            }
            else
            {
                TryCatchEventStack.Last().TriggerCatchEvent(ex);
            }
        }
        public void ThrowSilent(string message, bool once = false) => ThrowSilent(message, ExceptionType.CompilerException, once);
        public void ThrowSilent(string message, ExceptionType type = ExceptionType.CompilerException, bool once = false)
        {
            var ex = new ExceptionObject(message, type);
            if ((!once) || (once && _onceList.FirstOrDefault(f => f.Message == ex.Message) == null))
            {
                string msg = $"[SILENT ERROR] ({ex.Type.ToString()}) {ex.Message} File: {ex.Line}";
                LogWarn(msg);
                if (Settings.LogLevel == "warn")
                {
                    Manager.Print(msg, ConsoleColor.DarkYellow);
                }
                if (once)
                {
                    _onceList.Add(ex);
                }
            }
        }
    }
    public interface ITryCatchEvent
    {
        void TriggerCatchEvent(ExceptionObject ex);
    }
    /// <summary>
    /// Contains relevent data pertaining to the thrown exception
    /// </summary>
    public class ExceptionObject
    {
        public string Message { get; }
        public string Line { get; private set; }
        public string Snippet { get; private set; }
        public ExceptionType Type { get; }

        public ExceptionObject(string msg, ExceptionType type)
        {
            Message = msg; Type = type; SetLine();
        }
        private void SetLine()
        {
            string line = Manager.CurrentParsedLine;
            if (line != null && line.Contains("AnonymousFunction"))
            {
                var anonlist = Manager.AnonymousFunctionValueHolder;
                foreach (var x in anonlist)
                {
                    if (line.Contains(x.Name))
                        line = line.Replace("\"" + x.Name + "\"", "=" + x.Value);
                }
            }
            Snippet = line;
            if (line != null)
            {
                var firstLine = line.Split('\n').FirstOrDefault(f => !String.IsNullOrWhiteSpace(f));
                if (firstLine != null)
                {
                    foreach (var file in Manager.LoadedFileReference)
                    {
                        var sp = file.Value.Split('\n');
                        var index = 0;
                        foreach (var x in sp)
                        {
                            index++;
                            if (x != null && x.Contains(firstLine))
                                break;
                        }
                        if (index != sp.Length)
                        {
                            Line = $"{file.Key}:{index}";
                            break;
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Holds the value of anonymous functions so that the exception handler can grab it
    /// </summary>
    public class AnonymousFunctionValueHolder
    {
        public string Name { get; }
        public string Value { get; }
        public AnonymousFunctionValueHolder(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    public class TryCatchStack
    {
        private List<ITryCatchEvent> _tlist;
        public TryCatchStack()
        {
            _tlist = new List<ITryCatchEvent>();
        }
        public void Add(ITryCatchEvent item)
        {
            _tlist.Add(item);
        }
        public void RemoveLast()
        {
            _tlist.RemoveAt(_tlist.Count - 1);
        }
        public ITryCatchEvent Last()
        {
            return _tlist.LastOrDefault();
        }
    }
}
