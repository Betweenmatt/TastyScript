using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using TastyScript.ParserManager.Driver;
using TastyScript.ParserManager.ExceptionHandler;
using TastyScript.ParserManager.IOStream;
using TastyScript.ParserManager.Looping;

namespace TastyScript.ParserManager
{
    public class Manager
    {
        private static bool _isScriptStopping;
        private static int _anonymousFunctionIndex = -1;

        public static IDriver Driver { get; set; }

        public static string Title = $"TastyScript v{Assembly.GetExecutingAssembly().GetName().Version.ToString()} Beta";
        public static int AnonymousFunctionIndex
        {
            get
            {
                _anonymousFunctionIndex++;
                return _anonymousFunctionIndex;
            }
        }
        public static CancellationTokenSource CancellationTokenSource;
        
        public static LoopTracerList LoopTracerStack;

        public static string CurrentParsedLine { get; private set; }

        public static double SleepDefaultTime { get; set; }
        
        public static bool IsScriptStopping
        {
            get
            {
                return _isScriptStopping;
            }
            set
            {
                if (value && CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested)
                    CancellationTokenSource.Cancel();
                _isScriptStopping = value;
            }
        }
        
        public static IIOStream IOStream { get; private set; }
        
        public static IExceptionHandler ExceptionHandler;

        public static Dictionary<string, string> LoadedFileReference;

        public static List<AnonymousFunctionValueHolder> AnonymousFunctionValueHolder { get => anonymousFunctionValueHolder; set => anonymousFunctionValueHolder = value; }

        private static List<AnonymousFunctionValueHolder> anonymousFunctionValueHolder = new List<AnonymousFunctionValueHolder>();


        public static void Throw(string msg) => ExceptionHandler.Throw(msg);

        public static void Throw(string msg, ExceptionType type) => ExceptionHandler.Throw(msg,type);

        public static void ThrowSilent(string msg, bool once = true) => ExceptionHandler.ThrowSilent(msg, once);

        public static void ThrowSilent(string msg, ExceptionType type, bool once = true) => ExceptionHandler.ThrowSilent(msg, type, once);

        public static void ThrowDebug(string msg) => ExceptionHandler.ThrowDebug(msg);

        public static void Print(object msg, bool line = true) => IOStream.Print(msg,line);

        public static void Print(object msg, ConsoleColor color, bool line = true) => IOStream.Print(msg, color, line);

        public static string ReadLine() => IOStream.ReadLine();

        public static void SetCurrentParsedLine(string line) => CurrentParsedLine = line;

        public static void SetCancellationTokenSource() => CancellationTokenSource = new CancellationTokenSource();
        
        public static void Init(IIOStream io)
        {
            Settings.LoadSettings();
            
            IOStream = io;
        }
    }
}
