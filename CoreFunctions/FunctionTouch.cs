using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("Touch", new string[] { "intX", "intY", "sleep" })]
    public class FunctionTouch : FunctionDefinition
    {
        public override bool CallBase()
        {
            var x = (ProvidedArgs.First("intX"));
            var y = (ProvidedArgs.First("intY"));
            if (x == null || y == null)
            {
                Manager.Throw($"The function [{this.Name}] requires [{ExpectedArgs.Length}] arguments");
                return false;
            }
            double intX = 0;
            bool tryX = double.TryParse(x.ToString(), out intX);
            double intY = 0;
            bool tryY = double.TryParse(y.ToString(), out intY);
            if (!tryX || !tryY)
            {
                Manager.Throw($"Cannot get numbers from [{x.ToString()},{y.ToString()}]");
                return false;
            }
            if (!Manager.Driver.IsConnected())
                Manager.Print($"[DRIVERLESS] Touch x:{intX} y:{intY}");
            else
                Commands.Tap((int)intX, (int)intY);
            double sleep = Manager.SleepDefaultTime;
            if (ProvidedArgs.List.Count > 2)
            {
                sleep = 0;
                bool trySleep = double.TryParse(ProvidedArgs.First("sleep").ToString(), out sleep);
                if (!trySleep)
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
