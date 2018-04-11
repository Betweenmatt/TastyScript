using System;
using System.IO;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;

namespace TastyScript.CoreFunctions.Gui
{
    [Function("StopInvoke")]
    internal class FunctionStopInvoke : FunctionDefinition
    {
        public override bool CallBase()
        {
            StreamWriter streamWriter = FunctionInvoke.process.StandardInput;
            streamWriter.WriteLine("");
            return true;
        }
    }
}
