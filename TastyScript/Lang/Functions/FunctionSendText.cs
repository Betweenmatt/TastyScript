using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("SendText", new string[] { "s" })]
    internal class FunctionSendText : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "s");
            if (argsList == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, "Arguments cannot be null.", LineValue));

            if (Program.AndroidDriver != null)
            {
                Program.AndroidDriver.SendText(argsList.ToString());
            }
            else
            {
                IO.Output.Print($"[DRIVERLESS] text {argsList.ToString()}");
            }
            return args;
        }
    }
}
