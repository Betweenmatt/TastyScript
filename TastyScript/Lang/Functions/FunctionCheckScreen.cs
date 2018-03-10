using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("CheckScreen", new string[] { "succFunc", "failFunc", "succPath", "failPath" }, isSealed: true)]
    internal class FunctionCheckScreen : FDefinition
    {
        public override string CallBase()
        {
            var succFunc = ProvidedArgs.First("succFunc");
            var failFunc = ProvidedArgs.First("failFunc");
            var succPath = ProvidedArgs.First("succPath");
            var failPath = ProvidedArgs.First("failPath");
            if (succFunc == null || failFunc == null || succPath == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, $"Invoke function cannot be null.", LineValue));
            }
            var sf = FunctionStack.First(succFunc.ToString());
            var ff = FunctionStack.First(failFunc.ToString());
            if (sf == null || ff == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"[198]Invoke function cannot be found.", LineValue));
            }
            sf.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List);
            ff.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List);
            //check for threshold extension
            var threshExt = Extensions.FirstOrDefault(f => f.Name == "Threshold") as ExtensionThreshold;
            int thresh = 90;
            if (threshExt != null)
            {
                var param = threshExt.Extend();
                var nofail = int.TryParse(param[0].ToString(), out thresh);
            }
            if (failPath != null)
            {
                try
                {
                    Commands.AnalyzeScreen(succPath.ToString(), failPath.ToString(), sf, ff, thresh, this.Caller);
                }
                catch (Exception e)
                {
                    if (e is System.IO.FileNotFoundException)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                            $"File could not be found: {succPath.ToString()}, {failPath.ToString()}"));
                    }
                    Console.WriteLine(e);
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(""));
                }
            }
            else
            {
                try
                {
                    Commands.AnalyzeScreen(succPath.ToString(), sf, ff, thresh, this.Caller);
                }
                catch (Exception e)
                {
                    if (e is System.IO.FileNotFoundException)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                            $"File could not be found: {succPath.ToString()}"));
                    }
                    Console.WriteLine(e);
                    Compiler.ExceptionListener.Throw(new ExceptionHandler(""));
                }
            }

            return "";
        }
    }
}
