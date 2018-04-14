using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("Try", new string[] { "invoke" }, invoking: true, alias: new string[]{ "try" })]
    public class FunctionTry : FunctionDefinition
    {
        public override bool CallBase()
        {
            var tryBlock = ProvidedArgs.First("invoke");
            if (tryBlock == null)
                Manager.Throw("Function Try must have an invoked function");
            var tryfunc = FunctionStack.First(tryBlock.ToString());
            if (tryfunc == null)
                Manager.Throw($"Cannot find the invoked function.");

            var catchExt = Extensions.First("Catch");
            if (catchExt == null)
                Manager.Throw("Function Try must have `Catch` extension");
            var catchBlock = catchExt.Extend();
            if (catchBlock.ElementAtOrDefault(0) == null)
                Manager.Throw("Invoke for Catch block cannot be null");
            var catchfunc = FunctionStack.First(catchBlock[0].ToString());
            if (catchfunc == null)
                Manager.Throw($"Cannot find the invoked function.");
            Manager.ExceptionHandler.TryCatchEventStack.Add(new TryCatchEvent(tryfunc, catchfunc));
            
            new TFunction(tryfunc, this, tryfunc.GetInvokeProperties()).TryParse();

            return true;
        }
    }
}
