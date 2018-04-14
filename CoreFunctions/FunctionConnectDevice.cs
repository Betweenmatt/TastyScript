using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("ConnectDevice", new string[] { "serial" }, isSealed: true)]
    public class FunctionConnectDevice : FunctionDefinition
    {
        public override bool CallBase()
        {
            var argsList = ProvidedArgs.First("serial");
            string device = "";
            if (argsList?.ToString() == null || argsList.ToString() == "" || argsList.ToString() == "null")
                device = Commands.Connect();
            else
                device = Commands.Connect(argsList.ToString().UnCleanString());
            ReturnBubble = new Token("serial", device, "");
            return true;
        }
        protected override void ForExtension(BaseExtension findFor)
        {
            Manager.Throw($"Cannot call 'For' on {this.Name}.");
        }
    }
}
