using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("Sleep", new string[] { "time" })]
    internal class FunctionSleep : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var time = double.Parse((ProvidedArgs.FirstOrDefault(f => f.Name == "time") as TObject).Value.Value.ToString());
            //changed to utilities sleep for cancelation
            Utilities.Sleep((int)Math.Ceiling(time));
            return args;
        }
    }
}
