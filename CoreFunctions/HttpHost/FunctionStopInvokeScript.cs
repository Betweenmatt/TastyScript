using System;
using System.IO;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions.HttpHost
{
    [Function("StopInvokeScript")]
    public class FunctionStopInvokeScript : FunctionDefinition
    {
        public override bool CallBase()
        {
            StreamWriter streamWriter = Manager.GuiInvokeProcess.StandardInput;
            streamWriter.WriteLine("");
            return true;
        }
    }
}
