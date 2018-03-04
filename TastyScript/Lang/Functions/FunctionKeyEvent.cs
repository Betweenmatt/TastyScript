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
    internal class FunctionKeyEvent : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "keyevent");
            if (argsList == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, "Arguments cannot be null.", LineValue));

            if (Program.AndroidDriver != null)
            {
                //FunctionHelpers.AndroidBack();
                AndroidKeyCode newcol = AndroidKeyCode.A;
                var nofail = Enum.TryParse<AndroidKeyCode>(argsList.ToString(), out newcol);
                if (!nofail)
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                        $"The Key Event {argsList.ToString()} could not be found.", LineValue));
                Commands.KeyEvent(newcol);
            }
            else
            {
                IO.Output.Print($"[DRIVERLESS] Keyevent {argsList.ToString()}");
            }
            return args;
        }
    }
}
