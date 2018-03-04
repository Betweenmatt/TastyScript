using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Touch", new string[] { "intX", "intY", "sleep" })]
    internal class FunctionTouch : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var x = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX") as TObject);
            var y = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY") as TObject);
            if (x == null || y == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments", LineValue));
            }
            double intX = double.Parse(x.Value.Value.ToString());
            double intY = double.Parse(y.Value.Value.ToString());
            if (Program.AndroidDriver == null)
                IO.Output.Print($"[DRIVERLESS] Touch x:{intX} y:{intY}");
            else
                Commands.Tap((int)intX, (int)intY);
            double sleep = TokenParser.SleepDefaultTime;
            if (args.Value.Value.Count > 2)
            {
                sleep = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "sleep") as TObject).Value.Value.ToString());
            }
            FunctionHelpers.Sleep(sleep);
            return args;
        }
    }
}
