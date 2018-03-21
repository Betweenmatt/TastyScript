using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions.Internal
{
    [Function("This", new string[] { "type" }, alias:new string[] { "this" })]
    internal class FunctionThis : FDefinition
    {
        public override string CallBase()
        {
            /*
             * 
             * Since u always seem to forget, Caller.CallingFunction.Extensions to get the
             * Extensions hooked to the top level function this is being called from
             * 
             * */
            var prop = Extensions.FirstOrDefault(f => f.Name == "Prop") as ExtensionProp;
            var arg = ProvidedArgs.First("type");
            if (prop == null)
            {
                if (arg == null)
                    Compiler.ExceptionListener.Throw("Arguments cannot be null without a Prop extension");
                switch (arg.ToString())
                {
                    case ("Extension"):
                        List<string> output = new List<string>();
                        foreach (var x in Caller.CallingFunction.Extensions)
                            output.Add(x.Name);
                        ReturnBubble = new TArray("arr", output.ToArray(), "");
                        return "";
                    case ("UID")://ReadOnly
                        ReturnBubble = new Token("uid", UID.ToString(), "");
                        return "";
                    case ("IsAnonymous")://ReadOnly
                        ReturnBubble = new Token("isAnonymous", (IsAnonymous) ? "True" : "False", "");
                        return "";
                    case ("IsOverride")://ReadOnly
                        ReturnBubble = new Token("isOverride", (Override) ? "True" : "False", "");
                        return "";
                    case ("IsSealed")://ReadOnly
                        ReturnBubble = new Token("isSealed", (Sealed) ? "True" : "False", "");
                        return "";
                    case ("IsObsolete")://ReadOnly
                        ReturnBubble = new Token("isObsolete", (Obsolete) ? "True" : "False", "");
                        return "";
                }
            }
            else
            {
                var properties = prop.Extend();
                var fprop = properties.ElementAtOrDefault(0);
                if (fprop == null)
                    Compiler.ExceptionListener.Throw("Prop extension must have arguments");
                if (arg != null) {
                    switch (arg.ToString())
                    {
                        case ("Extension"):
                            if(fprop == "Args")
                            {
                                var sprop = properties.ElementAtOrDefault(1);
                                if (sprop == null)
                                    Compiler.ExceptionListener.Throw("Prop 'Args' must have a second parameter");
                                var get = Caller.CallingFunction.Extensions.FirstOrDefault(f=>f.Name == sprop);
                                if (get == null)
                                    ReturnBubble = new Token("null", "null", "");
                                else
                                    ReturnBubble = new TArray("arr", get.Extend(), "");
                            }
                            return "";
                    }
                }
            }
            return "";
        }
    }
}
