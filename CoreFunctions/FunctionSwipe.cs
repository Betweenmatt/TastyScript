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
            var x1 = (ProvidedArgs.First("intX1") );
            var y1 = (ProvidedArgs.First("intY1") );
            var x2 = (ProvidedArgs.First("intX2") );
            var y2 = (ProvidedArgs.First("intY2") );
            var dur = (ProvidedArgs.First("duration"));
            if (x1 == null || y1 == null || x2 == null || y2 == null || dur == null)
            {
                Manager.Throw($"The function [{this.Name}] requires [{ExpectedArgs.Length}] TNumber arguments");
                return false;
            }
            double intX1 = double.Parse(x1.ToString());
            double intY1 = double.Parse(y1.ToString());
            double intX2 = double.Parse(x2.ToString());
            double intY2 = double.Parse(y2.ToString());
            double duration = double.Parse(dur.ToString());
            if (!Manager.Driver.IsConnected())
                Manager.Print($"[DRIVERLESS] LongTouch x1:{intX1} y1:{intY1} x2:{intX2} y2:{intY2} duration:{duration}");
            else
                Commands.Swipe((int)intX1, (int)intY1, (int)intX2, (int)intY2, (int)duration);
            double sleep = Manager.SleepDefaultTime;
            if (ProvidedArgs.List.Count > 5)
            {
                sleep = double.Parse((ProvidedArgs.First("sleep")).ToString());
            }
            FunctionHelpers.Sleep(sleep, Caller);
            return true;
        }
    }
}
