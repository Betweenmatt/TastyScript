using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("AppPackage", new string[] { "app" }, isSealed: true)]
    public class FunctionAppPackage : FunctionDefinition
    {
        public override bool CallBase()
        {
            var print = "";
            var argsList = ProvidedArgs.First("app");
            if (argsList != null)
                print = argsList.ToString();
            if (!Manager.Driver.IsConnected())
            {
                Manager.Throw($"Cannot set the app package without having a device connected. Please connect to a device first.");
                return false;
            }
            ReturnBubble = new IFunction.Tokens.Token("appPkg", Commands.SetAppPackage(print),"");
            return true;
        }
    }
}
