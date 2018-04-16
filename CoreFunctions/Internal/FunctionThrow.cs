using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;
using TastyScript.ParserManager.ExceptionHandler;

namespace TastyScript.CoreFunctions.Internal
{
    [Function("Throw",new string[] { "msg" }, alias:new string[] { "throw" })]
    public class FunctionThrow : FunctionDefinition
    {
        public override bool CallBase()
        {
            string printmsg = "";
            var arg = ProvidedArgs.First("msg");
            if (arg != null)
                printmsg = arg.ToString();
            Manager.Throw(printmsg,ExceptionType.UserDefinedException);
            return true;
        }
    }
}
