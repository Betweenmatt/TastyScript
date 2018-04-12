using System;
using System.IO;
using System.Threading;
using TastyScript.ParserManager;
using TastyScript.ParserManager.Driver.Android;
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
                CancellationTokenSource source = new CancellationTokenSource();
                Thread esc = new Thread(()=> { ListenForEscape(source.Token); });
                esc.Start();
                StartScript(path, file);
                source.Cancel();
            }
            catch (Exception e)
            {
                //if loglevel is throw, then compilerControledException gets printed as well
                //only for debugging srs issues
                if (!(e is CompilerControlledException) || Settings.LogLevel == "throw")
                {
                    Manager.ExceptionHandler.LogThrow("Unexpected error", e);
                }
                //Console.WriteLine(e);
            }
        }
        public static void ListenForEscape(CancellationToken _cancelSource)
        {
            Manager.Print("Press ENTER to stop");
            
            var r = Reader.ReadLine(_cancelSource);

            //halt the script running in a child process
            if (Manager.GuiInvokeProcess != null)
            {
                StreamWriter streamWriter = Manager.GuiInvokeProcess.StandardInput;
                streamWriter.WriteLine("");
                //Manager.GuiInvokeProcess.Kill();
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
