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
            var func = new TFunction(sleep, caller.ParentFunction, ms.ToString());
            func.TryParse();
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
        /// <summary>
        /// Print to the iostream
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="line"></param>
        public void Print(string msg, bool line = true) => Manager.Print(msg, line);
        public void Print(string msg, ConsoleColor color, bool line = true) => Manager.Print(msg, color, line);

        public abstract bool CallBase();
        protected sealed override void TryParse()
        {
            var findFor = Extensions.First("For");
            if (findFor != null)
            {
                //if for extension exists, reroutes this tryparse method to the loop version without the for check
                ForExtension(findFor);
                return;
            }
            TryParse(true);
        }
        protected sealed override void TryParse(bool forFlag)
        {
            AssignParameters();
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
                if (!Manager.IsScriptStopping)
                {
                    if (Tracer == null || (!Tracer.Continue && !Tracer.Break))
                        return CallBase();

                }

                else if (Manager.IsScriptStopping && IsBlindExecute)
                {
                    return CallBase();
                }
            }
            return false;

        }
    }
}
