using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Functions
{
    [Function("Return", new string[]{"value"},isSealed:true, alias: new string[] { "return" })]
    internal class FunctionReturn : FDefinition
    {
        public override string CallBase()
        {
            var argsList = ProvidedArgs.First("value");
            if (argsList == null)
            {
                Caller.CallingFunction.ReturnBubble = new Tokens.Token("null", "null", "");
                Caller.CallingFunction.ReturnFlag = true;
                return "";
            }
            Caller.CallingFunction.ReturnBubble = argsList;
            Caller.CallingFunction.ReturnFlag = true;
            return "";
        }
    }
}
