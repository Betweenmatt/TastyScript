using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;

namespace TastyScript.CoreFunctions.Gui
{
    [Function("StopInvoke")]
    internal class FunctionStopInvoke : FunctionDefinition
    {
        public override bool CallBase()
        {
            FunctionInvoke.Test = true;
            return true;
        }
    }
}
