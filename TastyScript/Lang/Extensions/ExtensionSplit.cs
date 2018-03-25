using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    [Extension("Split",new string[] { "splitby" },varExtension:true)]
    [Serializable]
    internal class ExtensionSplit : EDefinition
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if (input == null)
                Compiler.ExceptionListener.Throw($"Cannot find Token [{input.Name}]",
                    ExceptionType.CompilerException, input.Line);
            if (args == null || args.ElementAtOrDefault(0) == null || args[0] == "")
            {
                //Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                //  ExceptionType.CompilerException, input.Line);
                var outstr = input.Value.UnCleanString().ToCharArray().Select(c => c.ToString()).ToArray();
                var outtok = new TArray("AnonArr", outstr, input.Line);
                return outtok;
            }
            else
            {
                var outstr = input.Value.Split(new string[] { args[0] }, StringSplitOptions.None);
                var outtok = new TArray("AnonArr", outstr, input.Line);
                return outtok;
            }
        }
    }
}
