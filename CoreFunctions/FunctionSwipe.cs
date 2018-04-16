using System;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("Swipe", new string[] { "intX1", "intY1", "intX2", "intY2", "duration", "sleep" })]
    public class FunctionSwipe : FunctionDefinition
    {
        public override bool CallBase()
        {
            var x1 = (ProvidedArgs.First("intX1"));
            var y1 = (ProvidedArgs.First("intY1"));
            var x2 = (ProvidedArgs.First("intX2"));
            var y2 = (ProvidedArgs.First("intY2"));
            var dur = (ProvidedArgs.First("duration"));
            if (x1 == null || y1 == null || x2 == null || y2 == null || dur == null)
            {
                Manager.Throw($"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments");
                return false;
            }
            if (!double.TryParse(x1.ToString(), out double intX1))
            {
                Throw($"Cannot get number from [{x1.ToString()}]");
                return false;
            }
            if (!double.TryParse(y1.ToString(), out double intY1))
            {
                Throw($"Cannot get number from [{y1.ToString()}]");
                return false;
            }
            if (!double.TryParse(x2.ToString(), out double intX2))
            {
                Throw($"Cannot get number from [{x2.ToString()}]");
                return false;
            }
            if (!double.TryParse(y2.ToString(), out double intY2))
            {
                Throw($"Cannot get number from [{y2.ToString()}]");
                return false;
            }
            if (!double.TryParse(dur.ToString(), out double duration))
            {
                Throw($"Cannot get number from [{dur.ToString()}]");
            }
            if (!Manager.Driver.IsConnected())
                Manager.Print($"[DRIVERLESS] Swipe x1:{intX1} y1:{intY1} x2:{intX2} y2:{intY2} duration:{duration}");
            else
                Commands.Swipe((int)intX1, (int)intY1, (int)intX2, (int)intY2, (int)duration);
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