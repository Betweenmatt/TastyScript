using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("Halt")]
    public class FunctionHalt : FunctionDefinition
    {
        public override bool CallBase()
        {
            return true;
        }
        protected override void ForExtension(TFunctionOld caller, BaseExtension findFor)
        {
            Manager.Throw($"Cannot call 'For' on {this.Name}.");
        }
    }
}
