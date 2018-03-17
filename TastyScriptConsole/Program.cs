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
using TastyScript;

namespace TastyScriptConsole
{
    internal class Program
    {
        
        private static CancellationTokenSource _cancelSource;
        private static string _consoleCommand = "";
        private static string Title = TastyScript.Main.Title;

        static void Main(string[] args)
        {
            //TastyScript.Main.DirectInit("../scripts/langex.ts", new IOStream());
            //return;//test

            TastyScript.Main.IO = new IOStream();
            TastyScript.Main.Init();
            if(Settings.RemoteToggle)
                StartRemote();
            Console.Title = Title;

            TastyScript.Main.IO.Print(WelcomeMessage());
            NewWaitForCommand();
        }
        private static void StartRemote()
        {
            TastyScript.Main.IO.Print("Starting remote listener.");
            Thread remote = new Thread(TcpListen);
            remote.Start();
        }
        public static void NewWaitForCommand()
        {
            while (true)
            {
                _consoleCommand = "";
                TastyScript.Main.IO.Print("\nSet your game to correct screen and then type run 'file/directory'\n", ConsoleColor.Green);
                TastyScript.Main.IO.Print('>', false);
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
                        TastyScript.Main.CommandExec(userInput);
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
                        TastyScript.Main.CommandRun(userInput);
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
                        TastyScript.Main.IO.Print("Enter '-h' for a list of commands!");
                        break;
                }
            }
        }

        private static void CommandADB(string r)
        {
            try
            {
                var cmd = r.Replace("adb ", "");
                TastyScript.Main.IO.Print("This command does not currently work as expected.");
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
                if (TastyScript.Main.AndroidDriver != null)
                {
                    TastyScript.Main.AndroidDriver.SetAppPackage(r);
                }
                else
                {
                    TastyScript.Main.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                }
            }
            catch (Exception e) { if (!(e is CompilerControledException) || Settings.LogLevel == "throw") { ExceptionListener.LogThrow("Unexpected error", e); } }
        }
        private static void CommandConnect(string r)
        {
            try
            {
                TastyScript.Main.AndroidDriver = new Driver(r);
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
            TastyScript.Main.IO.Print("Directory: " + Settings.QuickDirectory);
        }
        
        private static void CommandHelp(string r)
        {
            string output = $"Commands:\nrun 'path'\t\t|\tRuns the script at the given path\n" +
                $"connect 'serial'\t|\tConnects to the given device\n" +
                $"devices \t\t|\tLists all the devices connected to adb\n" +
                $"screenshot 'path'\t|\tTakes a screenshot of the device and\n\t\t\t\t saves it to the given path\n" +
                $"loglevel 'type'\t\t|\tSets the logging level to warn, error, or none" +
                $"app 'appPackage'\t\t|\tSets the current app package.";
            TastyScript.Main.IO.Print(output);
        }
        private static void CommandLogLevel(string r)
        {
            try
            {
                if (r == "")
                {
                    TastyScript.Main.IO.Print($"LogLevel: {Settings.LogLevel}");
                    return;
                }
                if (r == "warn" || r == "error" || r == "none" || r == "throw")
                {
                    Settings.SetLogLevel(r);
                    TastyScript.Main.IO.Print($"LogLevel: {Settings.LogLevel}");
                }
                else
                {
                    TastyScript.Main.IO.Print($"{r} is not a valid entry. Must be warn, error, throw, or none");
                }
            }
            catch
            {
                TastyScript.Main.IO.Print($"this is not a valid entry. Must be warn, error, throw, or none");
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
            TastyScript.Main.IO.Print("Remote Active: " + Settings.RemoteToggle);
        }
        
        
        private static void CommandScreenshot(string r)
        {
            try
            {
                if (TastyScript.Main.AndroidDriver != null)
                {
                    var ss = TastyScript.Main.AndroidDriver.GetScreenshot();
                    ss.Result.Save(r, ImageFormat.Png);
                }
                else
                {
                    TastyScript.Main.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                }
            }
            catch (Exception e) { if (!(e is CompilerControledException) || Settings.LogLevel == "throw") { ExceptionListener.LogThrow("Unexpected error", e); } }
        }
        private static void CommandShell(string r)
        {
            try
            {
                if (TastyScript.Main.AndroidDriver != null)
                {
                    TastyScript.Main.IO.Print($"Result: {TastyScript.Main.AndroidDriver.SendShellCommand(r.Replace("shell ", "").Replace("-sh ", ""))}");
                }
                else
                {
                    TastyScript.Main.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                }
            }
            catch (Exception e) { if (!(e is CompilerControledException) || Settings.LogLevel == "throw") { ExceptionListener.LogThrow("Unexpected error", e); } }
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
                            TastyScript.Main.SendStopScript();
                            break;
                    }
                }
                return next();
                
            });
        }
    }
}
