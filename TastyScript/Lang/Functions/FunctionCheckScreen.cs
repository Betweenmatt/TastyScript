using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Exceptions;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Token;

namespace TastyScript.Lang.Functions
{
    [Function("CheckScreen", new string[] { "succFunc", "failFunc", "succPath", "failPath" }, isSealed: true)]
    internal class FunctionCheckScreen : FDefinition<object>
    {
        public override object CallBase(TParameter args)
        {
            var succFunc = ProvidedArgs.FirstOrDefault(f => f.Name == "succFunc");
            var failFunc = ProvidedArgs.FirstOrDefault(f => f.Name == "failFunc");
            var succPath = ProvidedArgs.FirstOrDefault(f => f.Name == "succPath");
            var failPath = ProvidedArgs.FirstOrDefault(f => f.Name == "failPath");
            if (succFunc == null || failFunc == null || succPath == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, $"Invoke function cannot be null.", LineValue));
            }
            var sf = TokenParser.FunctionList.FirstOrDefault(f => f.Name == succFunc.ToString());
            var ff = TokenParser.FunctionList.FirstOrDefault(f => f.Name == failFunc.ToString());
            if (sf == null || ff == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"[198]Invoke function cannot be found.", LineValue));
            }
            //check for threshold extension
            var threshExt = Extensions.FirstOrDefault(f => f.Name == "Threshold") as ExtensionThreshold;
            int thresh = 90;
            if (threshExt != null)
            {
                var param = threshExt.Extend();
                var nofail = int.TryParse(param.Value.Value[0].ToString(), out thresh);
            }
            if (failPath != null)
            {
                try
                {
                    Commands.AnalyzeScreen(succPath.ToString(), failPath.ToString(), sf, ff, thresh, this);
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
                    Commands.AnalyzeScreen(succPath.ToString(), sf, ff, thresh, this);
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

            return args;
        }
    }
}
