using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Touch", new string[] { "intX", "intY", "sleep" })]
    internal class FunctionTouch : FDefinition
    {
        public override string CallBase()
        {
            var x = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX"));
            var y = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY"));
            if (x == null || y == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments", LineValue));
            }
            double intX = double.Parse(x.ToString());
            double intY = double.Parse(y.ToString());
            if (Program.AndroidDriver == null)
                IO.Output.Print($"[DRIVERLESS] Touch x:{intX} y:{intY}");
            else
                Commands.Tap((int)intX, (int)intY);
            double sleep = TokenParser.SleepDefaultTime;
            if (ProvidedArgs.Count > 2)
            {
                sleep = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "sleep")).ToString());
            }
            FunctionHelpers.Sleep(sleep);
            return "";
        }
    }
}
