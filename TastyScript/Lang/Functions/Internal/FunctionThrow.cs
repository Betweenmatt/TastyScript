using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Functions
{
    [Function("Throw",new string[] { "msg" })]
    internal class FunctionThrow : FDefinition
    {
        public override string CallBase()
        {
            string printmsg = "";
            var arg = ProvidedArgs.First("msg");
            if (arg != null)
                printmsg = arg.ToString();
            Compiler.ExceptionListener.Throw(printmsg,ExceptionType.UserCreatedException);
            return "";
        }
    }
}
