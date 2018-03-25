using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TastyScript.Lang.Functions
{
    [Function("ReadLine")]
    internal class FunctionReadLine : FDefinition
    {
        public override string CallBase()
        {
            string input = Main.IO.Read();
            ReturnBubble = new Tokens.Token("readline", input, "");
            return "";
        }
    }
}
