using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Functions.Gui
{
    [Function("StopInvoke")]
    internal class FunctionStopInvoke : FunctionDefinition
    {
        public override string CallBase()
        {
            Main.DirectStop();
            return "";
        }
    }
}
