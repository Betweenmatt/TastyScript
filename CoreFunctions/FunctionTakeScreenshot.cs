using System;
using System.Drawing.Imaging;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("TakeScreenshot", new string[] { "path" }, isSealed: true)]
    public class FunctionTakeScreenshot : FunctionDefinition
    {
        public override bool CallBase()
        {
            if (!Manager.Driver.IsConnected())
                Manager.Throw("Cannot take screenshot without a connected device");
            var path = ProvidedArgs.First("path");
            if (path == null)
            {
                Manager.Throw($"Path must be specified");
                return false;
            }
            var ss = Commands.GetScreenshot();
            try
            {
                ss.Save(path.ToString().UnCleanString(), ImageFormat.Png);
            }
            catch
            {
                Manager.ThrowSilent($"Unexpected error saving screenshot to path {path.ToString()}");
                ReturnBubble = new Token("bool", "null");
                return true;
            }
            ReturnBubble = new Token("bool", path.ToString());
            return true;
        }
    }
}
