using System;
using System.Diagnostics;
using System.Threading;
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
        internal static Process process;

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
                process = new Process();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "TastyScript.exe";
                process.StartInfo.Arguments = $"-r {funcname.ToString()} -d {Settings.QuickDirectory} -ll {Settings.LogLevel} -c {Manager.Driver.GetName()}";
                
                process.Start();
                ChildProcessTracker.AddProcess(process);
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }
                isInvoking = false;
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
    }
}
