using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("SendText", new string[] { "s" })]
    public class FunctionSendText : FunctionDefinition
    {
        public override bool CallBase()
        {
            var argsList = ProvidedArgs.First("s");
            if (argsList == null)
            {
                Manager.Throw("Arguments cannot be null.");
                return false;
            }
            if (Manager.Driver.IsConnected())
            {
                Commands.SendText(argsList.ToString());
            }
            else
            {
                Manager.Print($"[DRIVERLESS] text {argsList.ToString()}");
            }
            return true;
        }
    }
}
