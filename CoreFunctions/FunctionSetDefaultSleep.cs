using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("SetDefaultSleep", new string[] { "sleep" })]
    public class FunctionSetDefaultSleep : FunctionDefinition
    {
        public override bool CallBase()
        {
            var sleep = (ProvidedArgs.First("sleep"));
            double sleepnum = double.Parse(sleep.ToString());
            Manager.SleepDefaultTime = sleepnum;
            return true;
        }
        protected override void ForExtension(TFunction caller, BaseExtension findFor)
        {
            Manager.Throw($"Cannot call 'For' on {this.Name}.");
        }
    }
}
