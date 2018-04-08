using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Touch", new string[] { "intX", "intY", "sleep" })]
    internal class FunctionTouch : FunctionDefinition
    {
        public override string CallBase()
        {
            var x = (ProvidedArgs.First("intX"));
            var y = (ProvidedArgs.First("intY"));
            if (x == null || y == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] arguments", LineValue));
                return "";
            }
            double intX = 0;
            bool tryX = double.TryParse(x.ToString(), out intX);
            double intY = 0;
            bool tryY = double.TryParse(y.ToString(), out intY);
            if (!tryX || !tryY)
            {
                Compiler.ExceptionListener.Throw($"Cannot get numbers from [{x.ToString()},{y.ToString()}]");
                return "";
            }
            if (Main.AndroidDriver == null)
                Main.IO.Print($"[DRIVERLESS] Touch x:{intX} y:{intY}");
            else
                Commands.Tap((int)intX, (int)intY);
            double sleep = TokenParser.SleepDefaultTime;
            if (ProvidedArgs.List.Count > 2)
            {
                sleep = 0;
                bool trySleep = double.TryParse(ProvidedArgs.First("sleep").ToString(), out sleep);
                if (!trySleep)
                {
                    Compiler.ExceptionListener.Throw($"Cannot get numbers from [{ProvidedArgs.First("sleep").ToString()}]");
                    return "";
                }
            }
            FunctionHelpers.Sleep(sleep, Caller);
            return "";
        }
    }
}
