using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Continue", isSealed: true)]
    internal class FunctionContinue : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var tracer = Compiler.LoopTracerStack.LastOrDefault();
            if (tracer != null)
                tracer.SetContinue(true);
            else
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler($"Unexpected `Continue()` without a loop to trigger.", LineValue));
            return args;
        }
    }
}
