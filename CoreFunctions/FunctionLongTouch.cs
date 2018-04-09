using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("LongTouch", new string[] { "intX", "intY", "duration", "sleep" })]
    public class FunctionLongTouch : FunctionDefinition
    {
        public override bool CallBase()
        {
            var x = (ProvidedArgs.First("intX"));
            var y = (ProvidedArgs.First("intY"));
            var dur = (ProvidedArgs.First("duration"));
            if (x == null || y == null || dur == null)
            {
                Manager.Throw($"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments");
                return false;
            }
            double intX = double.Parse(x.ToString());
            double intY = double.Parse(y.ToString());
            double duration = double.Parse(dur.ToString());
            if (!Manager.Driver.IsConnected())
                Manager.Print($"[DRIVERLESS] LongTouch x:{intX} y:{intY} duration:{duration}");
            else
                Commands.LongTap((int)intX, (int)intY, (int)duration);
            double sleep = Manager.SleepDefaultTime;
            if (ProvidedArgs.List.Count > 3)
            {
                sleep = double.Parse((ProvidedArgs.First("sleep").ToString()));
            }
            FunctionHelpers.Sleep(sleep, Caller);
            return true;
        }
    }
}
