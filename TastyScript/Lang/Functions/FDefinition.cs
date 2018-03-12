using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    internal static class FunctionHelpers
    {
        public static void Sleep(double ms, TFunction caller)
        {
            var sleep = FunctionStack.First("Sleep");
            var func = new TFunction(sleep, new List<EDefinition>(), ms.ToString() , caller.CallingFunction);
            sleep.TryParse(func);
            //Utilities.Sleep((int)ms);
        }
        
    }
    internal class FDefinition : AnonymousFunction, IOverride
    {
        public virtual string CallBase() { return ""; }
        public override void TryParse(TFunction caller)
        {
            ReturnBubble = null;
            ReturnFlag = false;
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
                Caller = caller;
                Extensions = caller.Extensions;
            }
            var findFor = Extensions.FirstOrDefault(f => f.Name == "For") as ExtensionFor;
            if (findFor != null)
            {
                //if for extension exists, reroutes this tryparse method to the loop version without the for check
                ForExtension(caller, findFor);
                return;
            }
            if (caller != null && caller.Arguments != null && ExpectedArgs != null && ExpectedArgs.Length > 0)
            {
                
                ProvidedArgs = new TokenStack();
                var args = caller.ReturnArgsArray();
                if (args.Length > 0)
                {
                    if (args.Length > ExpectedArgs.Length)
                        Compiler.ExceptionListener.Throw($"The arguments supplied do not match the arguments expected!");
                    for (var i = 0; i < args.Length; i++)
                    {
                        var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                        ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                    }
                }
            }
            Parse();
        }
        public override void TryParse(TFunction caller, bool forFlag)
        {
            ReturnBubble = null;
            ReturnFlag = false;
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
                Caller = caller;
                Extensions = caller.Extensions;
            }
            if (caller != null && caller.Arguments != null && ExpectedArgs != null && ExpectedArgs.Length > 0)
            {
                ProvidedArgs = new TokenStack();
                var args = caller.ReturnArgsArray();
                if (args.Length > 0)
                {
                    if (args.Length > ExpectedArgs.Length)
                        Compiler.ExceptionListener.Throw($"The arguments supplied do not match the arguments expected!");
                    for (var i = 0; i < args.Length; i++)
                    {
                        var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                        ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                    }
                }
            }
            Parse();
        }

        public override string Parse()
        {
            if (Obsolete)
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.CompilerException,
                    $"The function [{this.Name}] is marked obsolete. Please check the documentation on future use of this function!", LineValue));
            }
            if (!ReturnFlag)
            {
                if (!TokenParser.Stop)
                {
                    if (Tracer == null || (!Tracer.Continue && !Tracer.Break))
                        return CallBase();

                }

                else if (TokenParser.Stop && BlindExecute)
                {
                    return CallBase();
                }
            }
            return "";

        }
    }
}
