using System.Threading;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;
using TastyScript.ParserManager.IOStream;

namespace TastyScript.CoreFunctions
{
    [Function("ReadLine")]
    public class FunctionReadLine : FunctionDefinition
    {
        public override bool CallBase()
        {
            Manager.StdInLine = "";
            while (!Manager.CancellationTokenSource.IsCancellationRequested 
                && Manager.StdInLine == "")
            {
                //sleep is only to prevent max cpu usage
                Thread.Sleep(100);
            }
            ReturnBubble = new Token("readline", Manager.StdInLine.CleanString());
            return true;
        }
    }
}
