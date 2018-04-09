using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("Base", isSealed: true)]
    public class FunctionBase : FunctionDefinition
    {
        public override void TryParse(TFunction caller)
        {
            Manager.Throw($"{this.Name} can not be overrided");
        }
        public override bool CallBase()
        {
            Manager.Throw($"{this.Name} can not be overrided.");
            return false;
        }
        protected override void ForExtension(TFunction caller, BaseExtension findFor)
        {
            Manager.Throw($"Cannot call 'For' on {this.Name}.");
        }
    }
}
