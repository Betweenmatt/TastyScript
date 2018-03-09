using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Break", isSealed: true, alias: new string[] { "break" })]
    internal class FunctionBreak : FDefinition
    {
        public override string CallBase()
        {
            var tracer = Compiler.LoopTracerStack.LastOrDefault();
            if (tracer != null)
                tracer.SetBreak(true);
            else
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler($"Unexpected `Break()` without a loop to trigger.", LineValue));
            return "";
        }
    }
}
