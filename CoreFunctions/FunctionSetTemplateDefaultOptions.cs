using System;
using System.Collections.Generic;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Functions;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;
using TastyScript.ParserManager.Driver.Android;

namespace TastyScript.CoreFunctions
{
    [Function("SetTemplateDefaultOptions", new string[] { "arr" })]
    public class FunctionSetTemplateDefaultOptions : FunctionDefinition
    {
        public override bool CallBase()
        {
            var arg = ProvidedArgs.First("arr");
            if (arg == null)
            {
                Manager.Throw($"Arguments for {this.Name} must not be null");
                return false;
            }
            var args = new TArray("", arg.ToString(), "");
            if (args.Arguments == null)
            {
                Manager.Throw($"Arguments for {this.Name} must be an array");
                return false;
            }
            var props = new List<string>();
            props.AddRange(args.Arguments);
            var ret = AnalyzeScreen.SetDefaultTemplateOptions(props.ToArray());
            ReturnBubble = new TArray("templateOptions", ret, "");
            return true;
        }
    }
}
