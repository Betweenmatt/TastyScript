using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;
using TastyScript.ParserManager.Driver.Android;

namespace TastyScript.CoreFunctions
{
    [Function("KeyEvent", new string[] { "keyevent" })]
    public class FunctionKeyEvent : FunctionDefinition
    {
        public override bool CallBase()
        {
            var argsList = ProvidedArgs.First("keyevent");
            if (argsList == null)
            {
                Manager.Throw("Arguments cannot be null.");
                return false;
            }

            if (!Manager.Driver.IsConnected())
            {
                //FunctionHelpers.AndroidBack();
                AndroidKeyCode newcol = AndroidKeyCode.A;
                var nofail = Enum.TryParse<AndroidKeyCode>(argsList.ToString().UnCleanString(), out newcol);
                if (!nofail)
                {
                    Manager.Throw($"The Key Event {argsList.ToString()} could not be found.");
                    return false;
                }
                Commands.KeyEvent(newcol);
            }
            else
            {
                Manager.Print($"[DRIVERLESS] Keyevent {argsList.ToString()}");
            }
            return true;
        }
    }
}
