using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("Return", new string[]{"value"},isSealed:true, alias: new string[] { "return" })]
    public class FunctionReturn : FunctionDefinition
    {
        public override bool CallBase()
        {
            var argsList = ProvidedArgs.First("value");
            if (argsList == null)
            {
                Caller.CallingFunction.ReturnToTopOfBubble(new Token("null", "null", ""));
                return true;
            }
            Caller.CallingFunction.ReturnToTopOfBubble(argsList);
            return true;
        }
    }
}
