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
using System.Net;
using System.Net.Sockets;
using Owin;
using Microsoft.Owin.Hosting;

namespace TastyScript
{
    class Program
    {
        public static Driver AndroidDriver;
        private static Thread QuickStop;
        private static List<IBaseFunction> predefinedFunctions;
        public static string Title = $"TastyScript v{Assembly.GetExecutingAssembly().GetName().Version.ToString()} Beta";
        public static string LogLevel;
        static void Main(string[] args)
        {
            LogLevel = Properties.Settings.Default.loglevel;
            Console.Title = Title;
            //on load set predefined functions and extensions to mitigate load from reflection
            predefinedFunctions = GetPredefinedFunctions();
            TokenParser.Extensions = GetExtensions();
            Compiler.ExceptionListener = new ExceptionListener();
            //
            IO.Output.Print(WelcomeMessage());
            WaitForCommand();
        }
        public static void WaitForCommand()
        {
            try
            {
                //kill the quickstop thread if it wasn't killed already
                if (QuickStop != null)
                    QuickStop.Abort();
            }
            catch { }
            IO.Output.Print("\nSet your game to correct screen and then type run 'file/directory'\n", ConsoleColor.Green);
            IO.Output.Print('>', false);
            var r = IO.Input.ReadLine();
            var split = r.Split(' ');

            switch (split[0])
            {
                case ("run"):
                    try
                    {
                        path = split[1].Replace("\'", "").Replace("\"", "");
                        try
                        {
                            file = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + path);
                        }
                        catch
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                                $"Could not find path: {path}"));
                            break;
                        }
                        TokenParser.SleepDefaultTime = 1200;
                        QuickStop = new Thread(ListenForEsc);
                        QuickStop.Start();
                        TokenParser.Stop = false;
                        StartScript();
                    }
                    catch (Exception e)
                    {
                        if (!(e is CompilerControledException))
                        {
                            //need a better way to handle this lol
                            IO.Output.Print(e, ConsoleColor.DarkRed);
                        }
                    }
                    break;
                case ("shell"):
                    try
                    {
                        if (AndroidDriver != null)
                        {
                            IO.Output.Print($"Result: {AndroidDriver.SendShellCommand(r.Replace("shell ",""))}");
                        }
                        else
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                        }
                    }
                    catch (Exception e) { if (!(e is CompilerControledException)) { IO.Output.Print(e, ConsoleColor.DarkRed); } }
                    break;
                case ("devices"):
                    Driver.PrintAllDevices();
                    break;
                case ("connect"):
                    try
                    {
                        AndroidDriver = new Driver(split[1]);
                    }
                    catch (Exception e) { if (!(e is CompilerControledException)) { IO.Output.Print(e, ConsoleColor.DarkRed); } }
                    break;
                case ("screenshot"):
                    try
                    {
                        if (AndroidDriver != null)
                        {
                            var ss = AndroidDriver.GetScreenshot();
                            ss.Save(split[1], ImageFormat.Png);
                        }
                        else
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                        }
                    }
                    catch (Exception e) { if (!(e is CompilerControledException)) { IO.Output.Print(e, ConsoleColor.DarkRed); } }
                    break;
                case ("app"):
                    try
                    {
                        if (AndroidDriver != null)
                        {
                            AndroidDriver.SetAppPackage(split[1]);
                        }
                        else
                        {
                            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException, "Device must be defined"));
                        }
                    }
                    catch (Exception e) { if (!(e is CompilerControledException)) { IO.Output.Print(e, ConsoleColor.DarkRed); } }
                    break;
                //this is not fully functional yet
                case ("remote"):
                    //TcpListen();
                    break;
                case ("loglevel"):
                    try
                    {
                        if (split[1] == "warn" || split[1] == "error" || split[1] == "none")
                        {
                            LogLevel = split[1];
                            Properties.Settings.Default.loglevel = split[1];
                        }
                        else
                        {
                            IO.Output.Print($"{split[1]} is not a valid entry. Must be warn, error, or none");
                        }
                    }catch
                    {
                        IO.Output.Print($"this is not a valid entry. Must be warn, error, or none");
                    }
                    break;
                case ("-h"):
                    IO.Output.Print(HelpMessage());
                    break;
                default:
                    IO.Output.Print("Enter -h for a list of commands!");
                    break;
            }
            
            WaitForCommand();
        }
        private static string path;
        private static string file;
        private static void StartScript()
        {
            Compiler c = new Compiler(path, file, predefinedFunctions);
        }
        private static void ListenForEsc()
        {
            IO.Output.Print("Press [ENTER KEY] to stop script execution");
            IO.Input.ReadLine();
            TokenParser.Stop = true;
            IO.Output.Print("\nScript execution is halting. Please wait.\n", ConsoleColor.Yellow);
            if (TokenParser.HaltFunction != null)
            {
                TokenParser.HaltFunction.BlindExecute = true;
                TokenParser.HaltFunction.TryParse(null);
            }
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
                            inst.SetProperties(attt.Name, attt.ExpectedArgs);
                            if (!attt.Obsolete)
                                temp.Add(inst);
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
                            inst.SetProperties(attt.Name, attt.ExpectedArgs);
                            if (!attt.Obsolete)
                                temp.Add(inst);
                        }
            return temp;
        }
        private static string WelcomeMessage()
        {
            return $"Welcome to {Title}!\nCredits:\n@TastyGod - https://github.com/TastyGod " +
                $"\nAforge - www.aforge.net\nSharpADB - https://github.com/quamotion/madb \n\n" + 
                $"Enter -h for a list of commands!\n";
        }
        private static string HelpMessage()
        {
            return $"Commands:\nrun 'path'\t\t|\tRuns the script at the given path\n"+
                $"connect 'serial'\t|\tConnects to the given device\n"+
                $"devices \t\t|\tLists all the devices connected to adb\n"+
                $"screenshot 'path'\t|\tTakes a screenshot of the device and\n\t\t\t\t saves it to the given path\n"+
                $"loglevel 'type'\t\t|\tSets the logging level to warn, error, or none"+
                $"app 'appPackage'\t\t|\tSets the current app package.";
        }
        /*
        public static void TcpListen()
        {
            const string Url = "http://localhost:8080/";
            using (WebApp.Start(Url, ConfigureApplication))
            {
                Console.WriteLine("Listening at {0}", Url);
                Console.WriteLine("Press [Enter] to close the Tcp Listener");
                Console.ReadLine();
            }
        }
        
        //this is not fully funcitonal yet
        private static void ConfigureApplication(IAppBuilder app)
        {
            app.Use((ctx, next) =>
            {
                //Console.WriteLine("Request \"{0}\" from: {1}:{2}",ctx.Request.Path,ctx.Request.RemoteIpAddress,ctx.Request.RemotePort);
                var split = ctx.Request.Path.ToString().Split('/');
                switch (split[1])
                {
                    case ("run"):
                        try
                        {
                            path = split[2].Replace("\'", "").Replace("\"", "").Replace("-","/");
                            try
                            {
                                file = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + path);
                            }
                            catch
                            {
                                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.SystemException,
                                    $"Could not find path: {path}"));
                                break;
                            }
                            TokenParser.SleepDefaultTime = 1200;
                            QuickStop = new Thread(ListenForEsc);
                            QuickStop.Start();
                            TokenParser.Stop = false;
                            StartScript();
                        }
                        catch (Exception e)
                        {
                            if (!(e is CompilerControledException))
                            {
                                //need a better way to handle this lol
                                IO.Output.Print(e, ConsoleColor.DarkRed);
                            }
                        }
                        break;
                    case ("stop"):
                        TokenParser.Stop = true;
                        IO.Output.Print("\nScript execution is halting. Please wait.\n", ConsoleColor.Yellow);
                        if (TokenParser.HaltFunction != null)
                        {
                            TokenParser.HaltFunction.BlindExecute = true;
                            TokenParser.HaltFunction.TryParse(null);
                        }
                        break;
                }
                return next();
            });
        }
        */
    }
}
