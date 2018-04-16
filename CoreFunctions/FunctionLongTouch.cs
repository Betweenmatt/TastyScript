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
            if (!double.TryParse(x.ToString(), out double intX))
            {
                Throw($"Cannot get number from [{x.ToString()}]");
                return false;
            }
            if (!double.TryParse(y.ToString(), out double intY))
            {
                Throw($"Cannot get number from [{y.ToString()}]");
                return false;
            }
            if (!double.TryParse(dur.ToString(), out double duration))
            {
                Throw($"Cannot get number from [{dur.ToString()}]");
            }
            if (!Manager.Driver.IsConnected())
                Manager.Print($"[DRIVERLESS] LongTouch x:{intX} y:{intY} duration:{duration}");
            else
                Commands.LongTap((int)intX, (int)intY, (int)duration);
            double sleep = Manager.SleepDefaultTime;
            if (ProvidedArgs.List.Count > 2 && ProvidedArgs.First("sleep")?.ToString() != "null")
            {
                if (!double.TryParse(ProvidedArgs.First("sleep").ToString(), out sleep))
                {
                    Manager.Throw($"Cannot get numbers from [{ProvidedArgs.First("sleep").ToString()}]");
                    return false;
                }
            }
            FunctionHelpers.Sleep(sleep, Caller);
            return true;
        }
    }
}