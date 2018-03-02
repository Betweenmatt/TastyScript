using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("ConnectDevice", new string[] { "serial" }, isSealed: true)]
    internal class FunctionConnectDevice : FDefinition<object>
    {

        public override object CallBase(TParameter args)
        {
            var print = "";
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "serial");
            if (argsList != null)
                print = argsList.ToString();
            Program.AndroidDriver = new Android.Driver(print);

            return args;
        }
        protected override void ForExtension(TParameter args, ExtensionFor findFor, string lineval)
        {
            Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException, $"Cannot call 'For' on {this.Name}.", LineValue));
        }
    }
}
