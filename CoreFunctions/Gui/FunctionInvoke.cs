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

namespace TastyScript.CoreFunctions.Gui
{
    [Function("Invoke", new string[] { "func", "postfunc" })]
    internal class FunctionInvoke : FunctionDefinition
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
            //var func = FunctionStack.First(funcname.ToString());
            //func.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List, Caller.CallingFunction.ProvidedArgs.List);
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
                    postfunc.TryParse(new TFunction(Caller.Function, new ExtensionList(), new string[] { }, this, null));
                }
            });
            st.Start();
            /*
            
            
            if(postfuncname != null)
            {
                var postfunc = FunctionStack.First(postfuncname.ToString());
                func.TryParse(new TFunction(Caller.Function, new ExtensionList(), new string[] { }, this, null));
            }*/
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
                                   line = lv1.Attribute("line").Value
                               };
                    foreach (var x in objs)
                    {
                        var func = FunctionStack.First("StdOut");
                        func.TryParse(new TFunction(func, new ExtensionList(), new string[] 
                        {
                            x.text.CleanString(),//.Replace("[","").Replace("]","").Replace(",","."),
                            x.line,
                            x.color
                        }, this));
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
                func.TryParse(new TFunction(func, new ExtensionList(), new string[] { msg, "true", "" }, this));
            }
        }
    }
}
