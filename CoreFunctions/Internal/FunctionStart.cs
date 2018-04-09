using System;
using System.Collections.Generic;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("Start")]
    public class FunctionStart : FunctionDefinition
    {
        public override bool CallBase()
        {
            return true;
        }
    }
}
