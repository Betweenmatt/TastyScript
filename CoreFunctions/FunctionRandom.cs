using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;

namespace TastyScript.CoreFunctions
{
    [Function("Random", new string[] { "min", "max" })]
    public class FunctionRandom : FunctionDefinition
    {
        public override bool CallBase()
        {
            var min = ProvidedArgs.First("min")?.ToString();
            var max = ProvidedArgs.First("max")?.ToString();
            int minnum = 0;
            int maxnum = 0;
            if (!int.TryParse(min, out minnum))
                minnum = 0;
            if (!int.TryParse(max, out maxnum))
                maxnum = 100;
            var random = new Random();
            var number = random.Next(minnum, maxnum);
            ReturnBubble = new IFunction.Tokens.Token("rng", number.ToString());
            return true;
        }
    }
}
