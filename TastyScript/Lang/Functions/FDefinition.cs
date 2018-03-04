using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TastyScript.Android;
using TastyScript.Lang.Extensions;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Functions
{
    internal static class FunctionHelpers
    {
        public static void Sleep(double ms)
        {
            /*
            var func = new TFunction("Sleep", TokenParser.FunctionList.FirstOrDefault(f => f.Name == "Sleep"));
            var newArgs = new TParameter("sleep", new List<IBaseToken>() { new TObject("sleep", ms) });
            func.Value.Value.TryParse(newArgs, null);
            */
            Utilities.Sleep((int)ms);
        }
    }
    internal class FDefinition : AnonymousFunction, IOverride
    {
        public virtual string CallBase() { return ""; }
        public override void TryParse(TFunction caller)
        {
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
                Caller = caller;
            }
            var findFor = Extensions.FirstOrDefault(f => f.Name == "For") as ExtensionFor;
            if (findFor != null)
            {
                //if for extension exists, reroutes this tryparse method to the loop version without the for check
                ForExtension(caller, findFor);
                return;
            }
            if (caller != null && caller.Arguments != null)
            {
                ProvidedArgs = new List<Token>();
                var args = caller.ReturnArgsArray();
                for (var i = 0; i < args.Length; i++)
                {
                    var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                    ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                }
            }
            Parse();
        }
        public override void TryParse(TFunction caller, bool forFlag)
        {
            if (caller != null)
            {
                BlindExecute = caller.BlindExecute;
                Tracer = caller.Tracer;
                Caller = caller;
            }
            if (caller != null && caller.Arguments != null)
            {
                ProvidedArgs = new List<Token>();
                var args = caller.ReturnArgsArray();
                for (var i = 0; i < args.Length; i++)
                {
                    var exp = ExpectedArgs[i].Replace("var ", "").Replace(" ", "");
                    ProvidedArgs.Add(new Token(exp, args[i], caller.Line));
                }
            }
            Parse();
        }

        public override string Parse()
        {
            if (!TokenParser.Stop)
            {
                if (Tracer == null || (!Tracer.Continue && !Tracer.Break))
                    return CallBase();

            }
            else if (TokenParser.Stop && BlindExecute)
            {
                return CallBase();
            }

            return "";

        }
    }
}
