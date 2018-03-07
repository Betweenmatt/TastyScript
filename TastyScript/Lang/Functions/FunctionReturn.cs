using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Functions
{
    [Function("Return", new string[]{"value"},isSealed:true)]
    internal class FunctionReturn : FDefinition
    {
        public override string CallBase()
        {
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "value");
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
