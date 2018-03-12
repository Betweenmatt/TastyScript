using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    [Extension("Replace", new string[] { "replace", "with" }, varExtension:true)]
    [Serializable]
    internal class ExtensionReplace : EDefinition
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if (input == null)
                Compiler.ExceptionListener.Throw($"Cannot find Token [{input.Name}]",
                    ExceptionType.CompilerException, input.Line);
            if (args == null || args.ElementAtOrDefault(0) == null || args.ElementAtOrDefault(1) == null)
                Compiler.ExceptionListener.Throw($"{this.Name} requires 2 arguments",
                    ExceptionType.CompilerException, input.Line);

            var outstr = input.Value.Replace(args[0], args[1]);
            var outtok = new Token("AnonStr", outstr, input.Line);
            return outtok;
        }
    }
}
