using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("Continue", isSealed: true, alias: new string[] { "continue" })]
    internal class FunctionContinue : FunctionDefinition
    {
        public override bool CallBase()
        {
            var tracer = Tracer;
            if (tracer != null)
                tracer.SetContinue(true);
            else
                Manager.ThrowSilent($"Unexpected `Continue()` without a loop to trigger.");
            return true;
        }
    }
}
