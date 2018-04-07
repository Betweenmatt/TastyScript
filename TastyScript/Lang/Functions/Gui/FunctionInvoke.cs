using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions.Gui
{
    [Function("Invoke", new string[] { "func", "postfunc" })]
    internal class FunctionInvoke : FDefinition
    {
        private static bool isInvoking;
        public override string CallBase()
        {
            if (isInvoking)
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler("There is already a function being invoked. Please wait for execution to stop before invoking another."));
                return null;
            }
            isInvoking = true;
            var funcname = ProvidedArgs.First("func");
            var postfuncname = ProvidedArgs.First("postfunc");
            var prop = Extensions.FirstOrDefault(f => f.Name == "Prop") as ExtensionProp;
            string[] passedargs = new string[] { };
            if (prop != null)
                passedargs = prop.Extend();

            if (funcname == null)
            {
                Compiler.ExceptionListener.Throw($"Invoke argument cannot be null");
                return null;
            }
            var func = FunctionStack.First(funcname.ToString());
            func.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List, Caller.CallingFunction.ProvidedArgs.List);
            func.TryParse(new TFunction(Caller.Function, new List<EDefinition>(), passedargs, this, null));
            isInvoking = false;
            if(TokenParser.Stop)TokenParser.Stop = false;
            Main.IO.Print("Invoke execution complete.", ConsoleColor.DarkGreen);
            if(postfuncname != null)
            {
                var postfunc = FunctionStack.First(postfuncname.ToString());
                func.TryParse(new TFunction(Caller.Function, new List<EDefinition>(), new string[] { }, this, null));
            }
            return "";
        }
    }
}
