using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("AppPackage", new string[] { "app" }, isSealed: true)]
    internal class FunctionAppPackage : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var print = "";
            var argsList = ProvidedArgs.FirstOrDefault(f => f.Name == "app");
            if (argsList != null)
                print = argsList.ToString();
            if (Program.AndroidDriver == null)
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                    $"Cannot set the app package without having a device connected. Please connect to a device first.", LineValue));
            Commands.SetAppPackage(print);
            return args;
        }
    }
}
