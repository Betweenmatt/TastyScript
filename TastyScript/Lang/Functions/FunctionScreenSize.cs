using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("ScreenSize")]
    internal class FunctionScreenSize : FDefinition
    {
        public override string CallBase()
        {
            ReturnBubble = new TArray("ScreenSize", new string[] { Main.AndroidDriver.ScreenWidth, Main.AndroidDriver.ScreenHeight }, "{0}");
            return "";
        }
    }
}
