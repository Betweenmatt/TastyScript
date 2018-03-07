using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("SendText", new string[] { "s" })]
    internal class FunctionSendText : FDefinition
    {
        public override string CallBase()
        {
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "s");
            if (argsList == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, "Arguments cannot be null.", LineValue));

            if (Program.AndroidDriver != null)
            {
                Commands.SendText(argsList.ToString());
            }
            else
            {
                IO.Output.Print($"[DRIVERLESS] text {argsList.ToString()}");
            }
            return "";
        }
    }
}
