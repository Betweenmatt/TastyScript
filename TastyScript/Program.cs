using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.ParserManager;
using TastyScript.ParserManager.Driver.Android;
using TastyScript.ParserManager.ExceptionHandler;

namespace TastyScript.TastyScript
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Manager.ExceptionHandler = new ExceptionHandler();
            Manager.Driver = new AndroidDriver();
            Manager.Init(new IOStream());
            string run = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-d")
                {
                    Settings.SetQuickDirectory(args[i + 1]);
                }
                else if(args[i] == "-r")
                {
                    run = args[i + 1];
                }
                else if(args[i] == "-ll")
                {
                    Settings.SetLogLevel(args[i + 1]);
                }
                else if(args[i] == "-c")
                {
                    if (args.ElementAtOrDefault(i + 1) != null && args[i + 1] != "")
                        Manager.Driver.Connect(args[i + 1]);
                }
                else if (args[i] == "-e")
                {
                    TastyScript.Main.CommandExec(args[i + 1]);
                    return;
                }
            }
            if(run == "")
            {
                Manager.Throw("Path cannot be empty.", ExceptionType.SystemException);
            }
            TastyScript.Main.CommandTestRun(run);
        }
    }
}
