using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("LongTouch", new string[] { "intX", "intY", "duration", "sleep" })]
    internal class FunctionLongTouch : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var x = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX") as TObject);
            var y = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY") as TObject);
            var dur = (ProvidedArgs.FirstOrDefault(f => f.Name == "duration") as TObject);
            if (x == null || y == null || dur == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments", LineValue));
            }
            double intX = double.Parse(x.Value.Value.ToString());
            double intY = double.Parse(y.Value.Value.ToString());
            double duration = double.Parse(dur.Value.Value.ToString());
            if (Program.AndroidDriver == null)
                IO.Output.Print($"[DRIVERLESS] LongTouch x:{intX} y:{intY} duration:{duration}");
            else
                Commands.LongTap((int)intX, (int)intY, (int)duration);
            double sleep = TokenParser.SleepDefaultTime;
            if (args.Value.Value.Count > 3)
            {
                sleep = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "sleep") as TObject).Value.Value.ToString());
            }
            FunctionHelpers.Sleep(sleep);
            return args;
        }
    }
}
