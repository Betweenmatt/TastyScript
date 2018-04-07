using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.ParserManager.ExceptionHandler;
using TastyScript.ParserManager.IOStream;

namespace TastyScript.ParserManager
{
    public class Manager
    {
        private static bool _guiScriptStopper;
        private static bool _scriptStopper;

        public static CancellationTokenSource CancellationTokenSource { get; private set; }

        public static double SleepDefaultTime { get; }

        /// <summary>
        /// All gui related functions get stopped by this bool
        /// </summary>
        public static bool GUIScriptStopper
        {
            get
            {
                return _guiScriptStopper;
            }
            set
            {
                if (value && CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested)
                    CancellationTokenSource.Cancel();
                _guiScriptStopper = value;
            }
        }
        
        /// <summary>
        /// All non-gui functions get stopped by this bool
        /// </summary>
        public static bool ScriptStopper
        {
            get
            {
                return _scriptStopper;
            }
            set
            {
                if (value && CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested)
                    CancellationTokenSource.Cancel();
                _scriptStopper = value;
            }
        }

        public static IIOStream IOStream { get; }

        public static IExceptionHandler ExceptionHandler { get; }

        //shortcuts to commonly used methods
        public static void Throw(string msg) => ExceptionHandler.Throw(msg);

        public static void Throw(ExceptionType type, string msg) => ExceptionHandler.Throw(type, msg);

        public static void ThrowSilent(string msg) => ExceptionHandler.ThrowSilent(msg);

        public static void ThrowSilent(ExceptionType type, string msg) => ExceptionHandler.ThrowSilent(type, msg);

        public static void ThrowDebug(string msg) => ExceptionHandler.ThrowDebug(msg);

        public static void Print(string msg, bool line = true) => IOStream.Print(msg,line);

        public static void Print(string msg, ConsoleColor color, bool line = true) => IOStream.Print(msg, color, line);

        public static string ReadLine() => IOStream.ReadLine();
    }
}
