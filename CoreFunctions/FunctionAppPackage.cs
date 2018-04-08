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
    internal class FunctionAppPackage : FunctionDefinition
    {
        public override string CallBase()
        {
            var print = "";
            var argsList = ProvidedArgs.First("app");
            if (argsList != null)
                print = argsList.ToString();
            if (Main.AndroidDriver == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                    $"Cannot set the app package without having a device connected. Please connect to a device first.", LineValue));
                return null;
            }
            Commands.SetAppPackage(print);
            return print;
        }
    }
}
