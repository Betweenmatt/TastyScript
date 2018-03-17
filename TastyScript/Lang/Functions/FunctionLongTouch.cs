using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("LongTouch", new string[] { "intX", "intY", "duration", "sleep" })]
    internal class FunctionLongTouch : FDefinition
    {
        public override string CallBase()
        {
            var x = (ProvidedArgs.First("intX"));
            var y = (ProvidedArgs.First("intY"));
            var dur = (ProvidedArgs.First("duration"));
            if (x == null || y == null || dur == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException,
                    $"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments", LineValue));
            }
            double intX = double.Parse(x.ToString());
            double intY = double.Parse(y.ToString());
            double duration = double.Parse(dur.ToString());
            if (Main.AndroidDriver == null)
                Main.IO.Print($"[DRIVERLESS] LongTouch x:{intX} y:{intY} duration:{duration}");
            else
                Commands.LongTap((int)intX, (int)intY, (int)duration);
            double sleep = TokenParser.SleepDefaultTime;
            if (ProvidedArgs.List.Count > 3)
            {
                sleep = double.Parse((ProvidedArgs.First("sleep").ToString()));
            }
            FunctionHelpers.Sleep(sleep, Caller);
            return "";
        }
    }
}
