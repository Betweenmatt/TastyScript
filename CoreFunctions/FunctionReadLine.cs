using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("ReadLine")]
    public class FunctionReadLine : FunctionDefinition
    {
        public override bool CallBase()
        {
            string input = Manager.ReadLine();
            ReturnBubble = new Token("readline", input, "");
            return true;
        }
    }
}
