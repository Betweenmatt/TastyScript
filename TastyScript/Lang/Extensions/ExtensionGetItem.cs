using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TastyScript.Lang.Tokens;

namespace TastyScript.Lang.Extensions
{
    /// <summary>
    /// Gets the item from the extended collection at the given index
    /// </summary>
    [Extension("GetItem", new string[] { "index" }, varExtension:true)]
    [Serializable]
    internal class ExtensionGetItem : EDefinition
    {
        public override Token Extend(Token input)
        {
            
            var args = Extend();
            if (args == null || args.ElementAtOrDefault(0) == null)
                Compiler.ExceptionListener.Throw($"{this.Name} arguments cannot be null.",
                    ExceptionType.CompilerException, input.Line);

            int index = 0;
            if (args.ElementAtOrDefault(0) != null)
            {
                var nofail = int.TryParse(args[0].ToString(), out index);
                if (!nofail)
                    Compiler.ExceptionListener.Throw($"{this.Name} arguments must be a whole number.",
                        ExceptionType.CompilerException, input.Line);
            }

            var inputAsTobj = new TArray("arr", input.Value, input.Line);
            if (inputAsTobj == null)
                Compiler.ExceptionListener.Throw($"Cannot find Token [{input.Name}]",
                    ExceptionType.CompilerException, input.Line);

            var ele = inputAsTobj.Arguments.ElementAtOrDefault(index);
            if (ele == null)
                Compiler.ExceptionListener.Throw($"The element at [{index}] is null.",
                    ExceptionType.NullReferenceException, input.Line);
            inputAsTobj.Arguments[index] = args[0];

            return new Token("arr", ele, input.Line);
        }
        
    }
}
