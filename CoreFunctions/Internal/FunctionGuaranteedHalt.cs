using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("GuaranteedHalt", isSealed: true)]
    internal class FunctionGuaranteedHalt : FunctionDefinition
    {
        public override bool CallBase()
        {
            FunctionTimer.TimerStop();
            return true;
        }
    }
}
