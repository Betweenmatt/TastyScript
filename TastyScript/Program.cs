using System;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using TastyScript.Android;
using TastyScript.Lang;
using TastyScript.Lang.Exceptions;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;

namespace TastyScript
{
    internal class Program
    {
        public static Driver AndroidDriver;
        private static List<IBaseFunction> predefinedFunctions;
        public static string Title = $"TastyScript v{Assembly.GetExecutingAssembly().GetName().Version.ToString()} Beta";
        private static CancellationTokenSource _cancelSource;
        private static string _consoleCommand = "";

        static void Main(string[] args) 
        {
            Settings.LoadSettings();
            if(Settings.RemoteToggle)
                StartRemote();
            Console.Title = Title;
            //on load set predefined functions and extensions to mitigate load from reflection
            predefinedFunctions = Utilities.GetPredefinedFunctions();
            Compiler.PredefinedList = predefinedFunctions;
            Utilities.GetExtensions();
            Compiler.ExceptionListener = new ExceptionListener();
            //
            IO.Output.Print(WelcomeMessage());
            NewWaitForCommand();
        }
        private static void StartRemote()
        {
            IO.Output.Print("Starting remote listener.");
            Thread remote = new Thread(TcpListen);
            remote.Start();
        }
        public static void NewWaitForCommand()
        {
            while (true)
            {
                _consoleCommand = "";
                IO.Output.Print("\nSet your game to correct screen and then type run 'file/directory'\n", ConsoleColor.Green);
                IO.Output.Print('>', false);
                var r = "";
                try
                {
                    _cancelSource = new CancellationTokenSource();
                    r = Reader.ReadLine(_cancelSource.Token);
                }
                catch (OperationCanceledException e)
                {}
                catch(Exception e)
                {
                    ExceptionListener.LogThrow("Unexpected error", e);
                }
                if (_consoleCommand != "")
                    r = _consoleCommand;

                var split = r.Split(' ');
                var userInput = "";
                if (split.Length > 1)
                    userInput = r.Replace(split[0] + " ", "");
                switch (split[0])
                {
                    case ("adb"):
                        CommandADB(userInput);
                        break;
                    case ("app"):
                        CommandApp(userInput);
                        break;
                    case ("-c"):
                    case ("connect"):
                        CommandConnect(userInput);
                        break;
                    case ("-d"):
                    case ("devices"):
                        CommandDevices(userInput);
                        break;
                    case ("dir"):
                        CommandDir(userInput);
                        break;
                    case ("-e"):
                    case ("exec"):
                        CommandExec(userInput);
                        break;
                    case ("-h"):
                    case ("help"):
                        CommandHelp(userInput);
                        break;
                    case ("-ll"):
                    case ("loglevel"):
                        CommandLogLevel(userInput);
                        break;
                    case ("remote"):
                        CommandRemote(userInput);
                        break;
                    case ("-r"):
                    case ("run"):
                        CommandRun(userInput);
                        break;
                    case ("-ss"):
                    case ("screenshot"):
                        CommandScreenshot(userInput);
                        break;
                    case ("-sh"):
                    case ("shell"):
                        CommandShell(userInput);
                        break;
                    default:
                        IO.Output.Print("Enter '-h' for a list of commands!");
                        break;
                }
            }
        }

        private static void CommandADB(string r)
        {
            try
            {
                var cmd = r.Replace("adb ", "");
                IO.Output.Print("This command does not currently work as expected.");
            }
            catch (Exception e)
            {
                ExceptionListener.LogThrow("Unexpected error", e);
            }
        }
        private static void CommandApp(string r)
        {
            try
            {
                if (AndroidDriver != null)
                {
                    AndroidDriver.SetAppPackage(r);
                }
                else
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                }
            }
            catch (Exception e) { if (!(e is CompilerControledException) || Settings.LogLevel == "throw") { ExceptionListener.LogThrow("Unexpected error", e); } }
        }
        private static void CommandConnect(string r)
        {
            try
            {
                AndroidDriver = new Driver(r);
            }
            catch (Exception e) { if (!(e is CompilerControledException) || Settings.LogLevel == "throw") { ExceptionListener.LogThrow("Unexpected error", e); } }
        }
        private static void CommandDevices(string r)
        {
            Driver.PrintAllDevices();
        }
        private static void CommandDir(string r)
        {
            if (r != "")
            {
                Settings.SetQuickDirectory(r);
                
            }
            IO.Output.Print("Directory: " + Settings.QuickDirectory);
        }
        private static void CommandExec(string r)
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
        private static void CommandHelp(string r)
        {
            string output = $"Commands:\nrun 'path'\t\t|\tRuns the script at the given path\n" +
                $"connect 'serial'\t|\tConnects to the given device\n" +
                $"devices \t\t|\tLists all the devices connected to adb\n" +
                $"screenshot 'path'\t|\tTakes a screenshot of the device and\n\t\t\t\t saves it to the given path\n" +
                $"loglevel 'type'\t\t|\tSets the logging level to warn, error, or none" +
                $"app 'appPackage'\t\t|\tSets the current app package.";
            IO.Output.Print(output);
        }
        private static void CommandLogLevel(string r)
        {
            try
            {
                if (r == "")
                {
                    IO.Output.Print($"LogLevel: {Settings.LogLevel}");
                    return;
                }
                if (r == "warn" || r == "error" || r == "none" || r == "throw")
                {
                    Settings.SetLogLevel(r);
                    IO.Output.Print($"LogLevel: {Settings.LogLevel}");
                }
                else
                {
                    IO.Output.Print($"{r} is not a valid entry. Must be warn, error, throw, or none");
                }
            }
            catch
            {
                IO.Output.Print($"this is not a valid entry. Must be warn, error, throw, or none");
            }
        }
        private static void CommandRemote(string r)
        {
            if (r != "")
            {
                var set = (r == "True" || r == "true") ? true : false;
                //check if remote was off before starting again
                if(!Settings.RemoteToggle && set)
                {
                    StartRemote();
                }
                Settings.SetRemoteToggle(set);
            }
            IO.Output.Print("Remote Active: " + Settings.RemoteToggle);
        }
        
        private static void CommandRun(string r)
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
        private static void CommandScreenshot(string r)
        {
            try
            {
                if (AndroidDriver != null)
                {
                    var ss = AndroidDriver.GetScreenshot();
                    ss.Result.Save(r, ImageFormat.Png);
                }
                else
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                }
            }
            catch (Exception e) { if (!(e is CompilerControledException) || Settings.LogLevel == "throw") { ExceptionListener.LogThrow("Unexpected error", e); } }
        }
        private static void CommandShell(string r)
        {
            try
            {
                if (AndroidDriver != null)
                {
                    IO.Output.Print($"Result: {AndroidDriver.SendShellCommand(r.Replace("shell ", "").Replace("-sh ", ""))}");
                }
                else
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                }
            }
            catch (Exception e) { if (!(e is CompilerControledException) || Settings.LogLevel == "throw") { ExceptionListener.LogThrow("Unexpected error", e); } }
        }

        private static void ListenForEscape()
        {
            IO.Output.Print("Press ENTER to stop");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter)
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
        private static void SendStopScript()
        {
            try
            {
                //halt the script
                TokenParser.Stop = true;
                IO.Output.Print("\nScript execution is halting. Please wait.\n", ConsoleColor.Yellow);
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
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.SystemException,
                    $"Unknown error with halt thread, aborting all execution."));
                TokenParser.Stop = true;
            }

        }

        private static bool StartScript(string path, string file)
        {
            Compiler c = new Compiler(path, file, predefinedFunctions);
            if(!TokenParser.Stop)
                SendStopScript();
            Thread.Sleep(2000);//sleep for 2 seconds after finishing the script
            return true;
        }

        
        private static string WelcomeMessage()
        {
            return $"Welcome to {Title}!\nCredits:\n@TastyGod - https://github.com/TastyGod " +
                $"\nAforge - www.aforge.net\nSharpADB - https://github.com/quamotion/madb" + 
                $"\nLog4Net - https://logging.apache.org/log4net \n\n" +
                $"Enter -h for a list of commands!\n";
        }
        
        public static void TcpListen()
        {
            const string Url = "http://localhost:8080/";
            using (WebApp.Start(Url, ConfigureApplication))
            {
                while (Settings.RemoteToggle) { };
            }
        }
        private static void ConfigureApplication(IAppBuilder app)
        {
            app.Use((ctx, next) =>
            {
                if (Settings.RemoteToggle)
                {
                    //remove pretext
                    var split = ctx.Request.Path.ToString().Split('/');
                    //get command from data
                    var cmd = split[1].Split('=');
                    //get data from data while keeping slashes
                    var r = ctx.Request.Path.ToString().Replace(split[0] + cmd[0] + "=", "");
                    switch (cmd[0])
                    {
                        case ("run"):
                            _consoleCommand = $"run {r}";
                            _cancelSource.Cancel();
                            break;
                        case ("stop"):
                            SendStopScript();
                            break;
                    }
                }
                return next();
                
            });
        }
    }
}
