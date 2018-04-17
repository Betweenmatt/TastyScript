using System;
using System.IO;
using System.Threading;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;
using TastyScript.ParserManager.Driver.Android;
using TastyScript.ParserManager.ExceptionHandler;
using TastyScript.ParserManager.IOStream;

namespace TastyScript.TastyScript
{
    public static class Main
    {
        public static void CommandExec(string r)
        {
            try
            {
                var cmd = r.Replace("exec ", "").Replace("-e ", "");
                var file = "override.Start(){\n" + cmd + "\n}";
                var path = "AnonExecCommand.ts";
                Manager.SleepDefaultTime = 1200;
                Manager.IsScriptStopping = false;
                StartScript(path, file);
            }
            catch (Exception e) { if (!(e is CompilerControlledException) || Settings.LogLevel == "throw") { Manager.ExceptionHandler.LogThrow("Unexpected error", e); } }

        }
        public static void CommandRun(string r)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            try
            {
                var path = r.Replace("\'", "").Replace("\"", "");
                var file = Utilities.GetFileFromPath(path);
                Manager.SleepDefaultTime = 1200;
                Manager.IsScriptStopping = false;
                new Thread(()=> { ListenForStdIn(source.Token); }).Start();
                StartScript(path, file);
                source.Cancel();
            }
            catch (Exception e)
            {
                source.Cancel();
                Manager.CancellationTokenSource.Cancel();
                //if loglevel is throw, then compilerControledException gets printed as well
                //only for debugging srs issues
                if (!(e is CompilerControlledException) || Settings.LogLevel == "throw")
                {
                    Manager.ExceptionHandler.LogThrow("Unexpected error", e);
                }
            }
        }
        /// <summary>
        /// Listens for the stdin and handles the data recieved.
        /// </summary>
        /// <param name="_cancelSource"></param>
        private static void ListenForStdIn(CancellationToken _cancelSource)
        {
            while (!_cancelSource.IsCancellationRequested)
            {
                var r = Reader.ReadLine(_cancelSource);
                var xml = XmlStreamObj.ReadStreamXml(r);
                if(xml != null)
                {
                    if (xml.Id == "PROCESS_SCRIPT_ESCAPE")
                        break;
                    else
                        Manager.StdInLine = xml.Text;
                }
                else if(r != "")
                {
                    Manager.ThrowSilent($"Input stream is not in the correct format and will be ignored: {r}");
                }
                else
                {
                    break;
                }
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
                    new TFunction(ScriptParser.HaltFunction).TryParse();
                }
                if (ScriptParser.GuaranteedHaltFunction != null)
                {
                    ScriptParser.GuaranteedHaltFunction.SetBlindExecute(true);
                    new TFunction(ScriptParser.GuaranteedHaltFunction).TryParse();
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

       
    }
}
