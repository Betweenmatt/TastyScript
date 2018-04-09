using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreExtensions.Variable
{
    [Extension("Replace", new string[] { "replace", "with" }, varExtension:true)]
    [Serializable]
    public class ExtensionReplace : BaseExtension
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if (input == null)
                Manager.Throw($"Cannot find Token [{input.Name}]");
            if (args == null || args.ElementAtOrDefault(0) == null || args.ElementAtOrDefault(1) == null)
                Manager.Throw($"{this.Name} requires 2 arguments");

            var outstr = input.Value.Replace(args[0], args[1]);
            var outtok = new Token("AnonStr", outstr, input.Line);
            return outtok;
        }
    }
}
