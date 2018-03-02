using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("GuaranteedHalt", isSealed: true)]
    internal class FunctionGuaranteedHalt : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            FunctionTimer.TimerStop();
            return args;
        }
    }
}
