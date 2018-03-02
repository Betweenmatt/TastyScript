using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("TakeScreenshot", new string[] { "path" }, isSealed: true)]
    internal class FunctionTakeScreenshot : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var path = ProvidedArgs.FirstOrDefault(f => f.Name == "path");
            if (path == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, $"Path must be specified", LineValue));
                return null;
            }
            var ss = Program.AndroidDriver.GetScreenshot();
            ss.Result.Save(path.ToString(), ImageFormat.Png);
            return args;
        }
    }
}
