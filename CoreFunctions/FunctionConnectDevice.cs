using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var print = "";
            var argsList = ProvidedArgs.First("serial");
            if (argsList != null)
                print = argsList.ToString();
            var dev = Commands.Connect(print);
            ReturnBubble = new Token("serial", dev, "");
            return true;
        }
        protected override void ForExtension(TFunction caller, BaseExtension findFor)
        {
            Manager.Throw($"Cannot call 'For' on {this.Name}.");
        }
    }
}
