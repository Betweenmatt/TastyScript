using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("Sleep", new string[] { "time" })]
    internal class FunctionSleep : FDefinition
    {
        public override string CallBase()
        {
            double time = 0;
            if (ProvidedArgs.First("time") != null)
            {
                bool tryfail = double.TryParse(ProvidedArgs.First("time").ToString(), out time);
                if (!tryfail)
                    time = TokenParser.SleepDefaultTime;
            }
            else
                time = TokenParser.SleepDefaultTime;
            //changed to utilities sleep for cancelations
            Utilities.Sleep((int)Math.Ceiling(time));
            return "";
        }
    }
}
