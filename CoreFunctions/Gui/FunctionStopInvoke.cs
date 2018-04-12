using System;
using System.IO;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.Gui
{
    [Function("StopInvoke")]
    internal class FunctionStopInvoke : FunctionDefinition
    {
        public override bool CallBase()
        {
            StreamWriter streamWriter = Manager.GuiInvokeProcess.StandardInput;
            streamWriter.WriteLine("");
            return true;
        }
    }
}
