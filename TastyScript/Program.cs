using System;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using TastyScript.Android;
using TastyScript.Lang.Func;
using TastyScript.Lang;

namespace TastyScript
{
    class Program
    {
        public static Driver AndroidDriver;
        private static Thread QuickStop;
        private static List<IBaseFunction> predefinedFunctions;
        public static string Title = $"TastyScript v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
        static void Main(string[] args)
        {
            Console.Title = Title;
            //on load set predefined functions and extensions to reduce load from reflection
            predefinedFunctions = GetPredefinedFunctions();
            TokenParser.Extensions = GetExtensions();
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
            IO.Output.Print("Set game to correct screen and then type run 'file/directory'", ConsoleColor.Green);
            IO.Output.Print('>', false);
            var r = IO.Input.ReadLine();
            var split = r.Split(' ');
            if (split[0] == "run")
            {
                try
                {
                    TokenParser.SleepDefaultTime = 1200;
                    QuickStop = new Thread(ListenForEsc);
                    QuickStop.Start();

                    TokenParser.Stop = false;
                    path = split[1].Replace("\'", "").Replace("\"", "");
                    file = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + split[1].Replace("\'", "").Replace("\"", ""));
                    StartScript();
                }
                catch (Exception e)
                {
                    IO.Output.Print(e, ConsoleColor.DarkRed);
                }
            }
            else if (split[0] == "devices")
            {
                Driver.PrintAllDevices();
            }
            else if (split[0] == "connect")
            {
                AndroidDriver = new Driver(split[1]);
            }
            else if (split[0] == "screenshot")
            {
                var ss = AndroidDriver.GetScreenshot();
                ss.Save(split[1], ImageFormat.Png);
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
            IO.Output.Print("Script execution is halting. Please wait.", ConsoleColor.Yellow);
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
            return $"Welcome to {Title}!\nCredits:\n@TastyGod\nAforge - www.aforge.net\nSharpADB - https://github.com/quamotion/madb \n\n" + 
                $"Enter -h for a list of commands!\n";
        }
    }
}
