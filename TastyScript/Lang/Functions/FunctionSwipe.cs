using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("Swipe", new string[] { "intX1", "intY1", "intX2", "intY2", "duration", "sleep" })]
    internal class FunctionSwipe : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var x1 = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX1") as TObject);
            var y1 = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY1") as TObject);
            var x2 = (ProvidedArgs.FirstOrDefault(f => f.Name == "intX2") as TObject);
            var y2 = (ProvidedArgs.FirstOrDefault(f => f.Name == "intY2") as TObject);
            var dur = (ProvidedArgs.FirstOrDefault(f => f.Name == "duration") as TObject);
            if (x1 == null || y1 == null || x2 == null || y2 == null || dur == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments", LineValue));
            }
            double intX1 = double.Parse(x1.Value.Value.ToString());
            double intY1 = double.Parse(y1.Value.Value.ToString());
            double intX2 = double.Parse(x2.Value.Value.ToString());
            double intY2 = double.Parse(y2.Value.Value.ToString());
            double duration = double.Parse(dur.Value.Value.ToString());
            if (Program.AndroidDriver == null)
                IO.Output.Print($"[DRIVERLESS] LongTouch x1:{intX1} y1:{intY1} x2:{intX2} y2:{intY2} duration:{duration}");
            else
                Commands.Swipe((int)intX1, (int)intY1, (int)intX2, (int)intY2, (int)duration);
            double sleep = TokenParser.SleepDefaultTime;
            if (args.Value.Value.Count > 5)
            {
                sleep = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "sleep") as TObject).Value.Value.ToString());
            }
            FunctionHelpers.Sleep(sleep);
            return args;
        }
    }
}
