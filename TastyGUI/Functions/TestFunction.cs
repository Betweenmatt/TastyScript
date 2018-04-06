using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript;
using TastyScript.Lang;
using TastyScript.Lang.Functions;

namespace TastyGUI.Functions
{
    [Function("TestFunction")]
    public class TestFunction : FDefinition
    {
        public override string CallBase()
        {
            Main.IO.Print("Hello, World!");
            return "";
        }
    }
}
