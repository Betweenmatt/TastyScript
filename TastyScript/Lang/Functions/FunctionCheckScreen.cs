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
    [Function("CheckScreen", new string[] { "succFunc", "failFunc", "succPath", "failPath" }, isSealed: true, isanon: false)]
    internal class FunctionCheckScreen : FDefinition
    {
        public override string CallBase()
        {
            if (Main.AndroidDriver == null)
            {
                Compiler.ExceptionListener.Throw("Cannot check screen without a connected device");
                return null;
            }
            var succFunc = ProvidedArgs.First("succFunc");
            var failFunc = ProvidedArgs.First("failFunc");
            var succPath = ProvidedArgs.First("succPath");
            var failPath = ProvidedArgs.First("failPath");
            if (succFunc == null || failFunc == null || succPath == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.NullReferenceException, $"Invoke function cannot be null.", LineValue));
                return null;
            }
            var sf = FunctionStack.First(succFunc.ToString());
            var ff = FunctionStack.First(failFunc.ToString());
            if (sf == null || ff == null)
            {
                Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.CompilerException,
                    $"[198]Invoke function cannot be found.", LineValue));
                return null;
            }
            sf.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List, Caller.CallingFunction.ProvidedArgs.List);
            ff.SetInvokeProperties(new string[] { }, Caller.CallingFunction.LocalVariables.List, Caller.CallingFunction.ProvidedArgs.List);
            //check for threshold extension
            var prop = CheckProperty();
            if (failPath != null)
            {
                try
                {
                    Commands.AnalyzeScreen(succPath.ToString(), failPath.ToString(), sf, ff, prop, this.Caller);
                }
                catch (Exception e)
                {
                    if (e is System.IO.FileNotFoundException)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                            $"File could not be found: {succPath.ToString()}, {failPath.ToString()}"));
                        return null;
                    }
                    Compiler.ExceptionListener.Throw(new ExceptionHandler("[57]Unexpected error with CheckScreen()"));
                    return null;
                }
            }
            else
            {
                try
                {
                    Commands.AnalyzeScreen(succPath.ToString(), sf, ff, prop, this.Caller);
                }
                catch (Exception e)
                {
                    if (e is System.IO.FileNotFoundException)
                    {
                        Compiler.ExceptionListener.Throw(new ExceptionHandler(ExceptionType.DriverException,
                            $"File could not be found: {succPath.ToString()}"));
                        return null;
                    }
                    Compiler.ExceptionListener.Throw(new ExceptionHandler("[73]Unexpected error with CheckScreen()"));
                    return null;
                }
            }

            return "";
        }
        private string[] CheckProperty()
        {
            var prop = Extensions.FirstOrDefault(f => f.Name == "Prop") as ExtensionProp;
            var save = Extensions.FirstOrDefault(f => f.Name == "Save") as ExtensionSave;
            if (prop != null)
            {
                var props = prop.Extend().ToList();
                if (save != null && save.Extend().ElementAtOrDefault(0) != null)
                    props.Add(save.Extend().ElementAtOrDefault(0));
                return props.ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}
