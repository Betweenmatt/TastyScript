using System;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("Sleep", new string[] { "time" })]
    public class FunctionSleep : FunctionDefinition
    {
        public override bool CallBase()
        {
            double time = 0;
            if (ProvidedArgs.First("time") != null)
            {
                bool tryfail = double.TryParse(ProvidedArgs.First("time").ToString(), out time);
                if (!tryfail)
                    time = Manager.SleepDefaultTime;
            }
            else
                time = Manager.SleepDefaultTime;
            //changed to utilities sleep for cancelations
            Utilities.Sleep((int)Math.Ceiling(time));
            return true;
        }
    }
}
