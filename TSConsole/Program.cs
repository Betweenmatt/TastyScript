using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.ParserManager;
using TastyScript.ParserManager.ExceptionHandler;
using TastyScript.ParserManager.IOStream;
using TastyScript.ParserManager.Driver.Android;

namespace TastyScript.TSConsole
{
    internal class Program
    {
        private static CancellationTokenSource _cancelSource;
        private static string _consoleCommand = "";
        private static string Title = Manager.Title;

        private static void Main(string[] args)
        {
            Manager.ExceptionHandler = new ExceptionHandler();
            Manager.Driver = new AndroidDriver();
            Manager.Init(new IOStream());
            Console.Title = Title;

            Manager.Print(WelcomeMessage());
            NewWaitForCommand();
        }

        public static void NewWaitForCommand()
        {
            while (true)
            {
                _consoleCommand = "";
                Manager.Print("\nSet your game to correct screen and then type run 'file/directory'\n", ConsoleColor.Green);
                Manager.Print(">", false);
                var r = "";
                try
                {
                    _cancelSource = new CancellationTokenSource();
                    r = Reader.ReadLine(_cancelSource.Token);
                }
                catch (OperationCanceledException e)
                { }
                catch (Exception e)
                {
                    Manager.ExceptionHandler.LogThrow("Unexpected error", e);
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
                        //CommandADB(userInput);
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
                        //TastyScript.Main.CommandExec(userInput);
                        break;

                    case ("-h"):
                    case ("help"):
                        CommandHelp(userInput);
                        break;

                    case ("-ll"):
                    case ("loglevel"):
                        CommandLogLevel(userInput);
                        break;

                    case ("-r"):
                    case ("run"):
                        var waitcancel = new CancellationTokenSource();
                        Thread th = new Thread(() =>
                        {
                            StartProcess(userInput);
                            waitcancel.Cancel();
                        });
                        th.Start();
                        Reader.ReadLine(waitcancel.Token);
                        StreamWriter myStreamWriter = process.StandardInput;
                        myStreamWriter.WriteLine("");
                        process.WaitForExit();
                        break;

                    case ("-ss"):
                    case ("screenshot"):
                        CommandScreenshot(userInput);
                        break;

                    case ("-sh"):
                    case ("shell"):
                        //CommandShell(userInput);
                        break;

                    default:
                        Manager.Print("Enter '-h' for a list of commands!");
                        break;
                }
            }
        }

        private static Process process;

        private static void StartProcess(string r)
        {
            process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Setup executable and parameters
            process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "TastyScript.exe";
            process.StartInfo.Arguments = $"-r {r} -d \"{Settings.QuickDirectory}\" -ll {Settings.LogLevel} -c \"{Manager.Driver.GetName()}\"";

            // Go
            process.Start();
            ChildProcessTracker.AddProcess(process);
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                (Manager.IOStream as IOStream).PrintXml(line);
            }
        }

        private static void CommandADB(string r)
        {
            try
            {
                var cmd = r.Replace("adb ", "");
                Manager.Print("This command does not currently work as expected.");
            }
            catch (Exception e)
            {
                Manager.ExceptionHandler.LogThrow("Unexpected error", e);
            }
        }

        private static void CommandApp(string r)
        {
            try
            {
                if (Manager.Driver != null)
                {
                    Manager.Driver.SetAppPackage(r);
                }
                else
                {
                    Manager.Throw("Device must be defined");
                }
            }
            catch (Exception e) { if (!(e is CompilerControlledException) || Settings.LogLevel == "throw") { Manager.ExceptionHandler.LogThrow("Unexpected error", e); } }
        }

        private static void CommandConnect(string r)
        {
            try
            {
                Manager.Driver.Connect(r);
            }
            catch (Exception e) { if (!(e is CompilerControlledException) || Settings.LogLevel == "throw") { Manager.ExceptionHandler.LogThrow("Unexpected error", e); } }
        }

        private static void CommandDevices(string r)
        {
            Manager.Driver.PrintAllDevices();
        }

        private static void CommandDir(string r)
        {
            if (r != "")
            {
                Settings.SetQuickDirectory(r);
            }
            Manager.Print("Directory: " + Settings.QuickDirectory);
        }

        private static void CommandHelp(string r)
        {
            string output = $"Commands:\nrun 'path'\t\t|\tRuns the script at the given path\n" +
                $"connect 'serial'\t|\tConnects to the given device\n" +
                $"devices \t\t|\tLists all the devices connected to adb\n" +
                $"screenshot 'path'\t|\tTakes a screenshot of the device and\n\t\t\t\t saves it to the given path\n" +
                $"loglevel 'type'\t\t|\tSets the logging level to warn, error, or none" +
                $"app 'appPackage'\t\t|\tSets the current app package.";
            Manager.Print(output);
        }

        private static void CommandLogLevel(string r)
        {
            try
            {
                if (r == "")
                {
                    Manager.Print($"LogLevel: {Settings.LogLevel}");
                    return;
                }
                if (r == "warn" || r == "error" || r == "none" || r == "throw")
                {
                    Settings.SetLogLevel(r);
                    Manager.Print($"LogLevel: {Settings.LogLevel}");
                }
                else
                {
                    Manager.Print($"{r} is not a valid entry. Must be warn, error, throw, or none");
                }
            }
            catch
            {
                Manager.Print($"this is not a valid entry. Must be warn, error, throw, or none");
            }
        }

        private static void CommandScreenshot(string r)
        {
            try
            {
                if (Manager.Driver != null)
                {
                    var ss = Manager.Driver.GetScreenshot();
                    ss.Result.Save(r, ImageFormat.Png);
                }
                else
                {
                    Manager.Throw("Device must be defined");
                }
            }
            catch (Exception e) { if (!(e is CompilerControlledException) || Settings.LogLevel == "throw") { Manager.ExceptionHandler.LogThrow("Unexpected error", e); } }
        }

        private static void CommandShell(string r)
        { }

        private static string WelcomeMessage()
        {
            return $"Welcome to {Title}!\nCredits:\n@TastyGod - https://github.com/TastyGod " +
                $"\nAforge - www.aforge.net\nSharpADB - https://github.com/quamotion/madb" +
                $"\nLog4Net - https://logging.apache.org/log4net \n\n" +
                $"Enter -h for a list of commands!\n";
        }
    }
}