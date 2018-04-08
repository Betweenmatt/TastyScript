using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Functions
{
    [Function("Return", new string[]{"value"},isSealed:true, alias: new string[] { "return" })]
    internal class FunctionReturn : FunctionDefinition
    {
        public override string CallBase()
        {
            var argsList = ProvidedArgs.First("value");
            if (argsList == null)
            {
                Caller.CallingFunction.ReturnToTopOfBubble(new Tokens.Token("null", "null", ""));
                return "";
            }
            Caller.CallingFunction.ReturnToTopOfBubble(argsList);
            return "";
        }
    }
}
