using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang;
using TastyScript.Lang.Exceptions;

namespace TastyScript
{
    public static class Main
    {
        public static Driver AndroidDriver;
        private static List<IBaseFunction> predefinedFunctions;
        public static string Title = $"TastyScript v{Assembly.GetExecutingAssembly().GetName().Version.ToString()} Beta";
        public static IIOStream IO;
        public static bool IsConsole = true;
        
        public static void Init()
        {

            Settings.LoadSettings();
            //on load set predefined functions and extensions to mitigate load from reflection
            predefinedFunctions = Utilities.GetPredefinedFunctions();
            Compiler.PredefinedList = predefinedFunctions;
            Utilities.GetExtensions();
            Compiler.ExceptionListener = new ExceptionListener();
            //
        }
        public static void Init(IExceptionListener el)
        {

            Settings.LoadSettings();
            //on load set predefined functions and extensions to mitigate load from reflection
            predefinedFunctions = Utilities.GetPredefinedFunctions();
            Compiler.PredefinedList = predefinedFunctions;
            Utilities.GetExtensions();
            Compiler.ExceptionListener = el;
            ExceptionListener.stupidFix = true;
            //
        }
        public static void Throw(ExceptionHandler e)
        {
            Compiler.ExceptionListener.Throw(e);
        }
        public static void ThrowSilent(ExceptionHandler e)
        {
            Compiler.ExceptionListener.ThrowSilent(e);
        }
        public static void CommandExec(string r)
        {
            try
            {
                var cmd = r.Replace("exec ", "").Replace("-e ", "");
                var file = "override.Start(){\n" + cmd + "}";
                var path = "AnonExecCommand.ts";
                TokenParser.SleepDefaultTime = 1200;
                TokenParser.Stop = false;
                StartScript(path, file);
            }
            catch (Exception e) { if (!(e is CompilerControledException) || Settings.LogLevel == "throw") { ExceptionListener.LogThrow("Unexpected error", e); } }
            
        }
        public static void CommandRun(string r)
        {
            try
            {
                var path = r.Replace("\'", "").Replace("\"", "");
                var file = Utilities.GetFileFromPath(path);
                TokenParser.SleepDefaultTime = 1200;
                TokenParser.Stop = false;
                Thread esc = new Thread(ListenForEscape);
                esc.Start();
                StartScript(path, file);

            }
            catch (Exception e)
            {
                //if loglevel is throw, then compilerControledException gets printed as well
                //only for debugging srs issues
                if (!(e is CompilerControledException) || Settings.LogLevel == "throw")
                {
                    ExceptionListener.LogThrow("Unexpected error", e);
                }
            }
        }
        public static void ListenForEscape()
        {
            TastyScript.Main.IO.Print("Press ENTER to stop");
            while (IO.ReadKey(true).Key != ConsoleKey.Enter)
            {
                if (TokenParser.Stop)
                    break;
            }
            if (!TokenParser.Stop)
            {
                SendStopScript();
            }
        }
        //stops the script, announces the halting and executes Halt() function if it exist
        public static void SendStopScript()
        {
            try
            {
                //halt the script
                TokenParser.Stop = true;
                TastyScript.Main.IO.Print("\nScript execution is halting. Please wait.\n", ConsoleColor.Yellow);
                if (TokenParser.HaltFunction != null)
                {
                    TokenParser.HaltFunction.BlindExecute = true;
                    TokenParser.HaltFunction.TryParse(null);
                }
                if (TokenParser.GuaranteedHaltFunction != null)
                {
                    TokenParser.GuaranteedHaltFunction.BlindExecute = true;
                    TokenParser.GuaranteedHaltFunction.TryParse(null);
                }
            }
            catch
            {
                TastyScript.Main.ThrowSilent(new ExceptionHandler(ExceptionType.SystemException,
                    $"Unknown error with halt thread, aborting all execution."));
                TokenParser.Stop = true;
            }

        }

        public static bool StartScript(string path, string file)
        {
            Compiler c = new Compiler(path, file, predefinedFunctions);
            if (!TokenParser.Stop)
                SendStopScript();
            Thread.Sleep(2000);//sleep for 2 seconds after finishing the script
            return true;
        }

        //these are for starting and stoppign the script from an external source like
        //notepad++
        public static void DirectInit(string f, string dir, IIOStream io, IExceptionListener listener)
        {
            IsConsole = false;
            IO = io;
            Init(listener);
            Settings.SetQuickDirectory(dir);
            Settings.SetLogLevel("warn");
            DirectRun(f);
        }
        private static void DirectRun(string r)
        {
            
            try
            {
                var path = r.Replace("\'", "").Replace("\"", "");
                var file = Utilities.GetFileFromPath(path);
                TokenParser.SleepDefaultTime = 1200;
                TokenParser.Stop = false;
                StartScript(path, file);
            }
            catch (Exception e)
            {
                //if loglevel is throw, then compilerControledException gets printed as well
                //only for debugging srs issues
                if (!(e is CompilerControledException) || Settings.LogLevel == "throw")
                {
                    ExceptionListener.LogThrow("Unexpected error", e);
                }
            }
        }
        public static void DirectStop()
        {
            if (!TokenParser.Stop)
            {
                SendStopScript();
            }
        }
    }
}
