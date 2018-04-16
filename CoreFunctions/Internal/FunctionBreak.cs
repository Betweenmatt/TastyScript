using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("Break", isSealed: true, alias: new string[] { "break" })]
    public class FunctionBreak : FunctionDefinition
    {
        public override bool CallBase()
        {
            Console.WriteLine(Tracer?.ID);
            var tracer = Tracer;
            if (tracer != null)
                tracer.SetBreak(true);
            else
                Manager.ThrowSilent($"Unexpected `Break()` without a loop to trigger.");
            return true;
        }
    }
}