using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    [Function("SetTemplateDefaultOptions", new string[] { "arr" })]
    internal class FunctionSetTemplateDefaultOptions : FDefinition
    {
        public override string CallBase()
        {
            var arg = ProvidedArgs.First("arr");
            if (arg == null)
                Compiler.ExceptionListener.Throw($"Arguments for {this.Name} must not be null");
            var args = new TArray("", arg.ToString(), "");
            if(args.Arguments == null)
                Compiler.ExceptionListener.Throw($"Arguments for {this.Name} must be an array");
            var props = new List<string>();
            props.AddRange(args.Arguments);
            var ret = AnalyzeScreen.SetDefaultTemplateOptions(props.ToArray());
            ReturnBubble = new TArray("templateOptions", ret, "");
            return "";
        }
    }
}
