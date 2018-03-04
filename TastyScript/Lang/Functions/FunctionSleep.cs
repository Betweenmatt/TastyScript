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
            var time = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "time")).ToString());
            //changed to utilities sleep for cancelations
            Utilities.Sleep((int)Math.Ceiling(time));
            return "";
        }
    }
}
