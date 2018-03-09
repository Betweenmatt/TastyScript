using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    [Extension("GetIndex",new string[] { "item" },varExtension:true)]
    [Serializable]
    internal class ExtensionGetIndex : EDefinition
    {
        public override Token Extend(Token input)
        {
            var args = Extend();
            if (args == null || args.ElementAtOrDefault(0) == null)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                    ExceptionType.CompilerException, input.Line);

            var inputAsTobj = new TArray("arr", input.Value, input.Line);
            if (inputAsTobj == null)
                Compiler.ExceptionListener.Throw($"Cannot find Token [{input.Name}]",
                    ExceptionType.CompilerException, input.Line);

            var ele = inputAsTobj.Arguments.FirstOrDefault(f => f == args[0]);
            if (ele == null)
                Compiler.ExceptionListener.Throw($"The element [{args[0]}] does not exist in this collection.",
                    ExceptionType.NullReferenceException, input.Line);
           // inputAsTobj.Arguments[index] = args[0];

            return new Token("index", Array.IndexOf(inputAsTobj.Arguments,args[0]).ToString(), input.Line);
        }
    }
}
