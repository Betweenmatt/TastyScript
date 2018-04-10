using System;
using System.Collections.Generic;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Function;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;
using TastyScript.ParserManager.ExceptionHandler;

namespace TastyScript.IFunction.Functions
{
    public static class FunctionHelpers
    {
        public static void Sleep(double ms, TFunction caller)
        {
            var sleep = FunctionStack.First("Sleep");
            var func = new TFunction(sleep, new ExtensionList(), ms.ToString() , caller.CallingFunction);
            sleep.TryParse(func);
        }
    }
    public abstract class FunctionDefinition : BaseFunction
    {
        /// <summary>
        /// Throws an exception to be handled by the parser.
        /// </summary>
        public void Throw(string msg) => Manager.Throw(msg);
        public void Throw(string msg, ExceptionType type) => Manager.Throw(msg, type);
        /// <summary>
        /// Throws a silent exception to be handled by the parser.
        /// </summary>
        public void ThrowSilent(string msg) => Manager.ThrowSilent(msg);
        public void ThrowSilent(string msg, ExceptionType type) => Manager.ThrowSilent(msg, type);

        public abstract bool CallBase();
        public sealed override void TryParse(TFunction caller)
        {
            ResetReturn();
            if (caller != null)
            {
                SetBlindExecute(caller.BlindExecute);
                Tracer = caller.Tracer;
                Caller = caller;
                Extensions = caller.Extensions;
            }
            var findFor = Extensions.First("For");
            if (findFor != null)
            {
                //if for extension exists, reroutes this tryparse method to the loop version without the for check
                ForExtension(caller, findFor);
                return;
            }
            if (caller != null && caller.Arguments != null && ExpectedArgs != null && ExpectedArgs.Length > 0)
            {
                
                ProvidedArgs = new TokenList();
                var args = caller.ReturnArgsArray();
                if (args.Length > 0)
                {
                    if (args.Length > ExpectedArgs.Length)
                    {
                        Manager.Throw($"The arguments supplied do not match the arguments expected!");
                        return;
                    }
                    for (var i = 0; i < args.Length; i++)
                    {
                        var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                        ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                    }
                }
            }
            Parse();
        }
        public sealed override void TryParse(TFunction caller, bool forFlag)
        {
            ResetReturn();
            if (caller != null)
            {
                SetBlindExecute(caller.BlindExecute);
                Tracer = caller.Tracer;
                Caller = caller;
                Extensions = caller.Extensions;
            }
            if (caller != null && caller.Arguments != null && ExpectedArgs != null && ExpectedArgs.Length > 0)
            {
                ProvidedArgs = new TokenList();
                var args = caller.ReturnArgsArray();
                if (args.Length > 0)
                {
                    if (args.Length > ExpectedArgs.Length)
                    {
                        Manager.Throw($"The arguments supplied do not match the arguments expected!");
                        return;
                    }
                    for (var i = 0; i < args.Length; i++)
                    {
                        var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                        ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                    }
                }
            }
            Parse();
        }

        private bool Parse()
        {
            if (IsObsolete)
            {
                Manager.ThrowSilent($"The function [{this.Name}] is marked obsolete. Please check the documentation on future use of this function!");
            }
            if (!ReturnFlag)
            {
                if ((IsGui && !Manager.IsGUIScriptStopping) || (!IsGui && !Manager.IsScriptStopping))
                {
                    if (Tracer == null || (!Tracer.Continue && !Tracer.Break))
                        return CallBase();

                }

                else if (((IsGui && Manager.IsGUIScriptStopping) || (!IsGui && Manager.IsScriptStopping)) && IsBlindExecute)
                {
                    return CallBase();
                }
            }
            return false;

        }
    }
}
