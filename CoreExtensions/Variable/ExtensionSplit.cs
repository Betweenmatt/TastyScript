using System;
using System.Collections.Generic;
using System.Linq;
using TastyScript.IFunction.Attributes;
using TastyScript.IFunction.Extension;
using TastyScript.IFunction.Tokens;
using TastyScript.ParserManager;

namespace TastyScript.CoreExtensions.Variable
{
    [Extension("Split",new string[] { "splitby" },varExtension:true)]
    [Serializable]
    public class ExtensionSplit : BaseExtension
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if (input == null)
                Manager.Throw($"Cannot find Token [{input.Name}]");
            if (args == null || args.ElementAtOrDefault(0) == null || args[0] == "")
            {
                //Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                //  ExceptionType.CompilerException, input.Line);
                var outstr = input.Value.UnCleanString().ToCharArray().Select(c => c.ToString()).ToArray();
                var outtok = new TArray("AnonArr", outstr);
                return outtok;
            }
            else
            {
                var outstr = input.Value.Split(new string[] { args[0] }, StringSplitOptions.None);
                var outtok = new TArray("AnonArr", outstr);
                return outtok;
            }
        }
    }
}
