using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Continue", isSealed: true, alias: new string[] { "continue" })]
    internal class FunctionContinue : FunctionDefinition
    {
        public override string CallBase()
        {
            var tracer = Tracer;
            if (tracer != null)
                tracer.SetContinue(true);
            else
                Compiler.ExceptionListener.ThrowSilent(
                    new ExceptionHandler($"Unexpected `Continue()` without a loop to trigger.", LineValue));
            return "";
        }
    }
}
