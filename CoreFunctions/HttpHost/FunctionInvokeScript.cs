using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.HttpHost
{
    [Function("InvokeScript", new string[] { "func", "postfunc" })]
    internal class FunctionInvokeScript : FunctionDefinition
    {
        private static bool isInvoking;

        public override bool CallBase()
        {
            if (isInvoking)
            {
                ThrowSilent("There is already a function being invoked. Please wait for execution to stop before invoking another.");
                return false;
            }
            isInvoking = true;
            var funcname = ProvidedArgs.First("func");
            var postfuncname = ProvidedArgs.First("postfunc");
            var prop = Extensions.First("Prop");
            string[] passedargs = new string[] { };
            if (prop != null)
                passedargs = prop.Extend();

            if (funcname == null)
            {
                Throw($"Invoke argument cannot be null");
                return false;
            }
            Thread st = new Thread(() =>
            {
                Manager.GuiInvokeProcess = new Process();
                Manager.GuiInvokeProcess.StartInfo.RedirectStandardOutput = true;
                Manager.GuiInvokeProcess.StartInfo.RedirectStandardInput = true;
                Manager.GuiInvokeProcess.StartInfo.UseShellExecute = false;
                Manager.GuiInvokeProcess.StartInfo.CreateNoWindow = true;
                Manager.GuiInvokeProcess.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "TastyScript.exe";
                Manager.GuiInvokeProcess.StartInfo.Arguments = $"-r {funcname.ToString().UnCleanString()} -d \"{Settings.QuickDirectory}\" -ll {Settings.LogLevel} -c \"{Manager.Driver.GetName()}\" -a \"{string.Join(",", passedargs)}\"";

                Manager.GuiInvokeProcess.Start();
                ChildProcessTracker.AddProcess(Manager.GuiInvokeProcess);
                while (!Manager.GuiInvokeProcess.StandardOutput.EndOfStream)
                {
                    string line = Manager.GuiInvokeProcess.StandardOutput.ReadLine();
                    SendToStdOut(line);
                }
                Manager.GuiInvokeProcess.WaitForExit();
                Print("Invoking function completed execution.", ConsoleColor.DarkGreen);
                isInvoking = false;
                if (postfuncname != null)
                {
                    var postfunc = FunctionStack.First(postfuncname.ToString());
                    var tfunc = new TFunction(postfunc, this);
                    tfunc.TryParse();
                    
                }
            });
            st.Start();
            return true;
        }
        private void SendToStdOut(string msg)
        {
            if (msg.Contains("<obj"))
            {
                try
                {
                    XDocument xdoc = XDocument.Parse(msg);

                    var objs = from lv1 in xdoc.Descendants("obj")
                               select new
                               {
                                   color = lv1.Attribute("color").Value,
                                   text = lv1.Attribute("text").Value,
                                   line = lv1.Attribute("line").Value,
                                   id = lv1.Attribute("id").Value
                               };
                    foreach (var x in objs)
                    {
                        var func = FunctionStack.First("StdOut");
                        var tfunc = new TFunction(func, this, new string[]
                        {
                            x.text.CleanString(),
                            x.line,
                            x.color,
                            x.id
                        });
                        tfunc.TryParse();
                    }
                }
                catch (Exception e)
                {
                    ThrowSilent($"Unknown error writing to stdout: {e.Message}", ExceptionType.SystemException);
                }
            }
            else
            {
                var func = FunctionStack.First("StdOut");
                new TFunction(func, this, new string[] { msg, "true", "" })
                    .TryParse();
            }
        }
    }
}
