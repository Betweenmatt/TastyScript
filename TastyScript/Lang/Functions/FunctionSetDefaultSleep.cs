using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("SetDefaultSleep", new string[] { "sleep" })]
    internal class FunctionSetDefaultSleep : FDefinition
    {
        public override string CallBase()
        {
            var sleep = (ProvidedArgs.FirstOrDefault(f => f.Name == "sleep"));
            double sleepnum = double.Parse(sleep.ToString());
            TokenParser.SleepDefaultTime = sleepnum;
            return "";
        }
        protected override void ForExtension(TFunction caller, ExtensionFor findFor)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
}
