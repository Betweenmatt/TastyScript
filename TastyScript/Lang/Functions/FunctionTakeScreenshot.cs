using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("TakeScreenshot", new string[] { "path" }, isSealed: true)]
    internal class FunctionTakeScreenshot : FDefinition
    {
        public override string CallBase()
        {
            if (Main.AndroidDriver == null)
                Compiler.ExceptionListener.Throw("Cannot take screenshot without a connected device");
            var path = ProvidedArgs.First("path");
            if (path == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, $"Path must be specified", LineValue));
                return null;
            }
            var ss = Commands.GetScreenshot();
            try
            {
                ss.Save(path.ToString(), ImageFormat.Png);
            }
            catch
            {
                Compiler.ExceptionListener.ThrowSilent(new ExceptionHandler(ExceptionType.CompilerException,
                    $"Unexpected error saving screenshot to path {path.ToString()}", ""));
                ReturnBubble = new Token("bool", "False", "");
                return "";
            }
            ReturnBubble = new Token("bool", "True","");
            return "";
        }
    }
}
