using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Containers;
using TastyScript.IFunction.Functions;
using TastyScript.ParserManager;

namespace TastyScript.CoreFunctions
{
    [Function("CheckScreen", new string[] { "succFunc", "failFunc", "succPath", "failPath" }, isSealed: true, isanon: false)]
    public class FunctionCheckScreen : FunctionDefinition
    {
        public override bool CallBase()
        {
            if (!Manager.Driver.IsConnected())
            {
                Manager.Throw("Cannot check screen without a connected device");
                return false;
            }
            var succFunc = ProvidedArgs.First("succFunc");
            var failFunc = ProvidedArgs.First("failFunc");
            var succPath = ProvidedArgs.First("succPath");
            var failPath = ProvidedArgs.First("failPath");
            if (succFunc == null || failFunc == null || succPath == null || succPath.ToString() == "null")
            {
                Manager.Throw($"Invoke function cannot be null.");
                return false;
            }
            var sf = FunctionStack.First(succFunc.ToString());
            var ff = FunctionStack.First(failFunc.ToString());
            if (sf == null || ff == null)
            {
                Manager.Throw($"[198]Invoke function cannot be found.");
                return false;
            }
            //check for threshold extension
            var prop = CheckProperty();
            if (failPath != null && failPath.ToString() != "null")
            {
                try
                {
                    Commands.AnalyzeScreen(succPath.ToString(), failPath.ToString(), sf, ff, prop, this.Caller);
                }
                catch (Exception e)
                {
                    if (e is System.IO.FileNotFoundException)
                    {
                        Manager.Throw($"File could not be found: {succPath.ToString()}, {failPath.ToString()}");
                        return false;
                    }
                    Manager.Throw(("[57]Unexpected error with CheckScreen()"));
                    return false;
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
                        Manager.Throw($"File could not be found: {succPath.ToString()}");
                        return false;
                    }
                    Manager.Throw(("[73]Unexpected error with CheckScreen()"));
                    return false;
                }
            }

            return true;
        }

        private string[] CheckProperty()
        {
            var prop = Extensions.First("Prop");
            var save = Extensions.First("Save");
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