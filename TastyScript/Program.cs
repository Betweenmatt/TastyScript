using System;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using TastyScript.Android;
using TastyScript.Lang.Func;
using TastyScript.Lang;
using TastyScript.Lang.Exceptions;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

namespace TastyScript
{
    class Program
    {
        public static Driver AndroidDriver;
        private static List<IBaseFunction> predefinedFunctions;
        public static string Title = $"TastyScript v{Assembly.GetExecutingAssembly().GetName().Version.ToString()} Beta";
        public static string LogLevel;
        private static CancellationTokenSource _cancelSource;
        private static string _quickDirectory;
        private static string _consoleCommand = "";
        private static bool _remoteActive;

        static void Main(string[] args) 
        {
            _quickDirectory = Properties.Settings.Default.dir;
            LogLevel = Properties.Settings.Default.loglevel;
            _remoteActive = Properties.Settings.Default.remote;
            if(_remoteActive)
                StartRemote();
            Console.Title = Title;
            //on load set predefined functions and extensions to mitigate load from reflection
            predefinedFunctions = GetPredefinedFunctions();
            TokenParser.Extensions = GetExtensions();
            Compiler.ExceptionListener = new ExceptionListener();
            //
            IO.Output.Print(WelcomeMessage());
            //WaitForCommand();
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

                //var r = IO.Input.ReadLine();
                var r = "";
                try
                {
                    _cancelSource = new CancellationTokenSource();
                    r = Reader.ReadLine(_cancelSource.Token);
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("thread has been poked");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
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
                //Driver.Test(cmd);
                IO.Output.Print("This command does not currently work as expected.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            catch (Exception e) { if (!(e is CompilerControledException || LogLevel == "throw")) { IO.Output.Print(e, ConsoleColor.DarkRed); } }
        }
        private static void CommandConnect(string r)
        {
            try
            {
                AndroidDriver = new Driver(r);
            }
            catch (Exception e) { if (!(e is CompilerControledException || LogLevel == "throw")) { IO.Output.Print(e, ConsoleColor.DarkRed); } }
        }
        private static void CommandDevices(string r)
        {
            Driver.PrintAllDevices();
        }
        private static void CommandDir(string r)
        {
            if (r != "")
            {
                _quickDirectory = r;
                Properties.Settings.Default.dir = _quickDirectory;
                Properties.Settings.Default.Save();
            }
            IO.Output.Print("Directory: " + _quickDirectory);
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
            catch (Exception e) { if (!(e is CompilerControledException || LogLevel == "throw")) { IO.Output.Print(e, ConsoleColor.DarkRed); } }

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
                    IO.Output.Print($"LogLevel: {LogLevel}");
                    return;
                }
                if (r == "warn" || r == "error" || r == "none" || r == "throw")
                {
                    LogLevel = r;
                    Properties.Settings.Default.loglevel = r;
                    Properties.Settings.Default.Save();
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
                var set = Properties.Settings.Default.remote = (r == "True" || r == "true") ? true : false;
                Properties.Settings.Default.Save();
                //check if remote was off before starting again
                if(!_remoteActive && set)
                {
                    StartRemote();
                }
                _remoteActive = set;
            }
            IO.Output.Print("Remote Active: " + _remoteActive);
        }
        
        private static void CommandRun(string r)
        {
            try
            {
                var path = r.Replace("\'", "").Replace("\"", "");
                var file = GetFileFromPath(path);
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
                if (!(e is CompilerControledException || LogLevel == "throw"))
                {
                    //need a better way to handle this lol
                    IO.Output.Print(e, ConsoleColor.DarkRed);
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
                    ss.Save(r, ImageFormat.Png);
                }
                else
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                }
            }
            catch (Exception e) { if (!(e is CompilerControledException || LogLevel == "throw")) { IO.Output.Print(e, ConsoleColor.DarkRed); } }
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
            catch (Exception e) { if (!(e is CompilerControledException || LogLevel == "throw")) { IO.Output.Print(e, ConsoleColor.DarkRed); } }
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
            //halt the script
            TokenParser.Stop = true;
            IO.Output.Print("\nScript execution is halting. Please wait.\n", ConsoleColor.Yellow);
            if (TokenParser.HaltFunction != null)
            {
                TokenParser.HaltFunction.BlindExecute = true;
                TokenParser.HaltFunction.TryParse(null);
            }
        }

        private static bool StartScript(string path, string file)
        {
            Compiler c = new Compiler(path, file, predefinedFunctions);
            TokenParser.Stop = true;
            return true;
        }

        //uses reflection to get all the IBaseFunction classes with the attribute [Function]
        private static List<IBaseFunction> GetPredefinedFunctions()
        {
            List<IBaseFunction> temp = new List<IBaseFunction>();
            string definedIn = typeof(Function).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                // Note that we have to call GetName().Name.  Just GetName() will not work.  The following
                // if statement never ran when I tried to compare the results of GetName().
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Function), true).Length > 0)
                        {
                            var func = System.Type.GetType(type.ToString());
                            var inst = Activator.CreateInstance(func) as IBaseFunction;
                            var attt = type.GetCustomAttribute(typeof(Function), true) as Function;
                            inst.SetProperties(attt.Name, attt.ExpectedArgs,attt.Invoking,attt.Sealed);
                            if (!attt.Obsolete)
                                temp.Add(inst);
                        }
            return temp;
        }
        //i guess a quick way to essentially deep clone base functions on demand.
        //idk how much this will kill performance but i cant think of another way
        public static IBaseFunction CopyFunctionReference(string funcName)
        {
            IBaseFunction temp = null;
            string definedIn = typeof(Function).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Function), true).Length > 0)
                        {
                            var attt = type.GetCustomAttribute(typeof(Function), true) as Function;
                            if (attt.Name == funcName)
                            {
                                var func = System.Type.GetType(type.ToString());
                                var inst = Activator.CreateInstance(func) as IBaseFunction;
                                inst.SetProperties(attt.Name, attt.ExpectedArgs, attt.Invoking, attt.Sealed);
                                if (!attt.Obsolete)
                                    temp = (inst);
                            }
                        }
            return temp;
        }
        private static List<IExtension> GetExtensions()
        {
            List<IExtension> temp = new List<IExtension>();
            string definedIn = typeof(Extension).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                // Note that we have to call GetName().Name.  Just GetName() will not work.  The following
                // if statement never ran when I tried to compare the results of GetName().
                if ((!assembly.GlobalAssemblyCache) && ((assembly.GetName().Name == definedIn) || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn)))
                    foreach (System.Type type in assembly.GetTypes())
                        if (type.GetCustomAttributes(typeof(Extension), true).Length > 0)
                        {
                            var func = System.Type.GetType(type.ToString());
                            var inst = Activator.CreateInstance(func) as IExtension;
                            var attt = type.GetCustomAttribute(typeof(Extension), true) as Extension;
                            inst.SetProperties(attt.Name, attt.ExpectedArgs,attt.Invoking);
                            if (!attt.Obsolete)
                                temp.Add(inst);
                        }
            return temp;
        }
        /// <summary>
        /// Checks both absolute and relative, as well as pre-set directories
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileFromPath(string path)
        {
            var file = "";
            //check if its a full path
            if (File.Exists(path))
                file = System.IO.File.ReadAllText(path);
            //check if the path is local to the app directory
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + path))
                file = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + path);
            //check for quick directory
            else if (File.Exists(_quickDirectory + "/" + path))
                file = System.IO.File.ReadAllText(_quickDirectory + "/" + path);
            //check for quick directory
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + _quickDirectory + "/" + path))
                file = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/" + _quickDirectory + "/" + path);
            //or fail
            else
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                    $"Could not find path: {path}"));
            }
            return file;
        }
        public static Bitmap GetImageFromPath(string path)
        {
            Bitmap file = null;
            if (File.Exists(path))
                file = (Bitmap)Bitmap.FromFile(path);
            else if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + path))
                file = (Bitmap)Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + path);
            else if (File.Exists(_quickDirectory + "/" + path))
                file = (Bitmap)Bitmap.FromFile(_quickDirectory + "/" + path);
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + _quickDirectory + "/" + path))
                file = (Bitmap)Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + "/" + _quickDirectory + "/" + path);
            else
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                    $"Could not find path: {path}"));
            }
            return file;
        }
        private static string WelcomeMessage()
        {
            return $"Welcome to {Title}!\nCredits:\n@TastyGod - https://github.com/TastyGod " +
                $"\nAforge - www.aforge.net\nSharpADB - https://github.com/quamotion/madb \n\n" + 
                $"Enter -h for a list of commands!\n";
        }
        // /*
        public static void TcpListen()
        {
            const string Url = "http://localhost:8080/";
            using (WebApp.Start(Url, ConfigureApplication))
            {
                //Console.WriteLine("Press [Esc] to close the Tcp Listener");
                //while(Console.ReadKey(true).Key != ConsoleKey.Escape) { }
                while (_remoteActive) { };
            }
        }
        //this is not fully funcitonal yet
        private static void ConfigureApplication(IAppBuilder app)
        {
            app.Use((ctx, next) =>
            {
                if (_remoteActive)
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
        //*/
    }
}
