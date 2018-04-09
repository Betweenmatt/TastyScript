using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("ScreenSize")]
    public class FunctionScreenSize : FunctionDefinition
    {
        public override bool CallBase()
        {
            if (!Manager.Driver.IsConnected())
            {
                Manager.Throw("Cannot get screen size without a connected device");
                return false;
            }
            ReturnBubble = new TArray("ScreenSize", new string[] { Commands.GetScreenWidth(), Commands.GetScreenHeight() }, "{0}");
            return true;
        }
    }
}
