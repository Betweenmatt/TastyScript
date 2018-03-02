using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("SetDefaultSleep", new string[] { "sleep" })]
    internal class FunctionSetDefaultSleep : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var sleep = (ProvidedArgs.FirstOrDefault(f => f.Name == "sleep") as TObject);
            double sleepnum = double.Parse(sleep.ToString());
            TokenParser.SleepDefaultTime = sleepnum;
            return args;
        }
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
}
