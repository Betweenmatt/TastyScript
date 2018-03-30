using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("KeyEvent", new string[] { "keyevent" })]
    internal class FunctionKeyEvent : FDefinition
    {
        public override string CallBase()
        {
            var argsList = ProvidedArgs.First("keyevent");
            if (argsList == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, "Arguments cannot be null.", LineValue));
                return null;
            }

            if (Main.AndroidDriver != null)
            {
                //FunctionHelpers.AndroidBack();
                AndroidKeyCode newcol = AndroidKeyCode.A;
                var nofail = Enum.TryParse<AndroidKeyCode>(argsList.ToString().UnCleanString(), out newcol);
                if (!nofail)
                {
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                        $"The Key Event {argsList.ToString()} could not be found.", LineValue));
                    return null;
                }
                Commands.KeyEvent(newcol);
            }
            else
            {
                Main.IO.Print($"[DRIVERLESS] Keyevent {argsList.ToString()}");
            }
            return "";
        }
    }
}
