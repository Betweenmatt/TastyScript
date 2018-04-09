using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.ParserManager;
using TastyScript.ParserManager.ExceptionHandler;
using TastyScript.ParserManager.IOStream;

namespace TastyScript.TastyScript
{
    public static class Main
    {
        public static bool IsConsole = true;
        
        public static void CommandExec(string r)
        {
            try
            {
                var cmd = r.Replace("exec ", "").Replace("-e ", "");
                var file = "override.Start(){\n" + cmd + "}";
                var path = "AnonExecCommand.ts";
                Manager.SleepDefaultTime = 1200;
                Manager.IsScriptStopping = false;
                StartScript(path, file);
            }
            catch (Exception e) { if (!(e is CompilerControlledException) || Settings.LogLevel == "throw") { Manager.ExceptionHandler.LogThrow("Unexpected error", e); } }

        }
        public static void CommandRun(string r)
        {
            try
            {
                var path = r.Replace("\'", "").Replace("\"", "");
                var file = Utilities.GetFileFromPath(path);
                Manager.SleepDefaultTime = 1200;
                Manager.IsScriptStopping = false;
                Manager.IsGUIScriptStopping = false;
                Thread esc = new Thread(ListenForEscape);
                esc.Start();
                StartScript(path, file);

            }
            catch (Exception e)
            {
                //if loglevel is throw, then compilerControledException gets printed as well
                //only for debugging srs issues
                if (!(e is CompilerControlledException) || Settings.LogLevel == "throw")
                {
                    Manager.ExceptionHandler.LogThrow("Unexpected error", e);
                }
                Console.WriteLine(e);
            }
        }
        public static void ListenForEscape()
        {
            Manager.Print("Press ENTER to stop");
            while (Manager.IOStream.ReadKey(true).Key != ConsoleKey.Enter)
            {
                if (Manager.IsScriptStopping)
                    break;
            }
            if (!Manager.IsScriptStopping)
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
                Manager.IsScriptStopping = true;
                Manager.IsGUIScriptStopping = true;
                Manager.Print("\nScript execution is halting. Please wait.\n", ConsoleColor.Yellow);
                if (ScriptParser.HaltFunction != null)
                {
                    ScriptParser.HaltFunction.SetBlindExecute(true);
                    ScriptParser.HaltFunction.TryParse(null);
                }
                if (ScriptParser.GuaranteedHaltFunction != null)
                {
                    ScriptParser.GuaranteedHaltFunction.SetBlindExecute(true);
                    ScriptParser.GuaranteedHaltFunction.TryParse(null);
                }
            }
            catch
            {
                Manager.ThrowSilent($"Unknown error with halt thread, aborting all execution.");
                Manager.IsScriptStopping = true;
                Manager.IsGUIScriptStopping = true;
            }

        }

        public static bool StartScript(string path, string file)
        {
            ScriptParser c = new ScriptParser(path, file);
            if (!Manager.IsScriptStopping)
                SendStopScript();
            Thread.Sleep(2000);//sleep for 2 seconds after finishing the script
            return true;
        }

        //these are for starting and stoppign the script from an external source like
        //notepad++
        public static void DirectInit(string f, string dir, string ll, IIOStream io, IExceptionHandler listener)
        {
            IsConsole = false;
            Manager.Init(io);
            Settings.SetQuickDirectory(dir);
            Settings.SetLogLevel(ll);
            DirectRun(f);
        }
        private static void DirectRun(string r)
        {

            try
            {
                var path = r.Replace("\'", "").Replace("\"", "");
                var file = Utilities.GetFileFromPath(path);
                Manager.SleepDefaultTime = 1200;
                Manager.IsScriptStopping = false;
                StartScript(path, file);
            }
            catch (Exception e)
            {
                //if loglevel is throw, then compilerControledException gets printed as well
                //only for debugging srs issues
                if (!(e is CompilerControlledException) || Settings.LogLevel == "throw")
                {
                    Manager.ExceptionHandler.LogThrow("Unexpected error", e);
                }
            }
        }
        public static void DirectStop()
        {
            if (!Manager.IsScriptStopping)
            {
                SendStopScript();
            }
        }
    }
}
